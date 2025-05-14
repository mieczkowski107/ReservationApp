using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Areas.Customer.Controllers;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using Stripe;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "CompanyManager,Admin")]
public class AppointmentController(IUnitOfWork unitOfWork, INotificationService notificationService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int? Id)
    {
        var appointment = unitOfWork.Appointments.Get(u => u.Id == Id, includeProperties: "Service.Company,User");
        if (appointment == null)
        {
            return NotFound();
        }
       
        if (!IsAuthorized(appointment, User))
        {
            return NotFound();
        }

        var companyAppointment = CompanyAppointmentsVM.MapFromAppointment(appointment);

        if (appointment.Service!.IsPrepaymentRequired!)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == Id);
            if (payment != null)
            {
                companyAppointment.PaymentStatus = payment.Status;
                companyAppointment.PaymentIntentId = payment.PaymentIntentId;
                return View(companyAppointment);
            }
        }
        return View(companyAppointment);
    }

    // By default, the appointment status is confirmed
    // If someone's forgot to mark the appointment as completed, it will be marked as completed by background job
    public IActionResult CompleteAppointment(int id)
    {
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id, includeProperties: "Service.Company,User", tracked: true);
        if (appointment == null)
        {
            return NotFound();
        }

        if (!IsAuthorized(appointment, User))
        {
            return NotFound();
        }

        if (appointment.AppointmentDateTime < DateTime.UtcNow)
        {
            appointment.Status = AppointmentStatus.Completed;
            TempData["success"] = "Appointment mark as completed succesfully!";
            unitOfWork.Save();
        }
        else
        {
            TempData["error"] = "You can make as completed after starting the appointment";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // Mark as no show appointment
    // It will be used when the user didn't show up
    // It must be marked as no show by the company manager
    public IActionResult MarkAsNoShowAppointment(int id)
    {
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id, includeProperties: "Service.Company,User");
        if (appointment == null)
        {
            return NotFound();
        }

        if (!IsAuthorized(appointment, User))
        {
            return NotFound();
        }

        if (appointment.AppointmentDateTime < DateTime.UtcNow)
        {
            appointment.Status = AppointmentStatus.NoShow;
            TempData["success"] = "Appointment mark as no show succesfully!";
            unitOfWork.Save();
        }
        else
        {
            TempData["error"] = "You can make as no show after starting the appointment";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // Cancel the appointment
    public IActionResult CancelAppointment(int id)
    {
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id,
                                                      includeProperties: "Service.Company,User",
                                                      tracked: true);
        if (appointment == null)
        {
            return NotFound();
        }

        if (!IsAuthorized(appointment, User))
        {
            return NotFound();
        }

        if (!appointment.IsCancelationAvailable())
        {
            TempData["error"] = "Appointment already passed or it is too late to cancel appointment";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (appointment.Service!.IsPrepaymentRequired)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if (payment is null)
            {
                return NotFound();
            }
          
            if (payment.Status == PaymentStatus.Succeeded)
            {
                return RedirectToAction("AppointmentRefund", "Payment", new {Area = "Customer",appointmentId = id });
            }
        }
        else
        {
            appointment.Status = AppointmentStatus.Cancelled;
            TempData["success"] = "Your appointment has been cancelled.";
        }

        //Hangfire 
        var notification = unitOfWork.Notification.Get(u => u.AppointmentId == id && u.Type == NotificationType.Reminder && u.Status != NotificationStatus.Sent, tracked: true);

        notificationService.CreateNotification(NotificationType.Cancellation, appointment.Id);
        var jobId = BackgroundJob.Enqueue(() => notificationService.SendNotification(appointment.Id));
        if (notification != null)
        {
            BackgroundJob.ContinueJobWith(jobId, () => unitOfWork.Notification.Remove(notification));
        }

        unitOfWork.Save();
        return RedirectToAction(nameof(Details), new { id });
    }

    #region APICALLS
    public  IActionResult GetCompanyAppointments(int Id)
    {
        var userId = UserService.GetUserId(User);
        var isManager = UserService.IsCompanyManager(User);

        var appointments =  unitOfWork.Appointments.GetAll(
            u => u.Service.CompanyId == Id &&
                 (!isManager || u.Service.Company.OwnerId == userId),
            includeProperties: "Service.Company,User").ToList();

        var companyAppointments = appointments
            .Select(CompanyAppointmentsVM.MapFromAppointment)
            .ToList();

        return Json(new { data = companyAppointments });
    }

    #endregion
    private bool IsAuthorized(Appointment appointment, ClaimsPrincipal User)
    {
        var userId = UserService.GetUserId(User);
        bool isAuthorized = UserService.IsAdmin(User) || appointment.Service!.Company!.OwnerId == userId;

        if (!isAuthorized)
        {
            return false;
        }
        return true;
    }
}
