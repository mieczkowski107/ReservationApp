using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Utility.Enums;
using Stripe;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "CompanyManager,Admin")]
public class AppointmentController(IUnitOfWork unitOfWork) : Controller
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
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (appointment.Service.Company.OwnerId != userId)
            {
                return NotFound();
            }
        }
        var companyAppointment = new CompanyAppointments()
        {
            AppointmentId = appointment.Id,
            CompanyId = appointment.Service!.CompanyId,
            CompanyName = appointment.Service?.Company?.Name,
            Date = appointment.Date,
            Time = appointment.Time,
            AppointmentStatus = appointment.Status,
            DurationMinutes = appointment.Service!.DurationMinutes,
            Price = appointment.Service.Price,
            ServiceName = appointment.Service.Name,
            UserFirstName = appointment.User!.FirstName,
            UserLastName = appointment.User.LastName,
            UserEmail = appointment.User.Email,
            UserPhoneNumber = appointment.User.PhoneNumber,
            IsPrepaymentRequired = appointment.Service.IsPrepaymentRequired,
        };

        if (!appointment.Service.IsPrepaymentRequired) return View(companyAppointment);
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == Id);
            if (payment == null) return View(companyAppointment);
            companyAppointment.PaymentStatus = payment.Status;
            companyAppointment.PaymentIntentId = payment.PaymentIntentId;
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
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (appointment.Service.Company.OwnerId != userId)
            {
                return NotFound();
            }
        }
        var now = DateTime.UtcNow;
        var appointmentDateTime = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second);
        if (appointmentDateTime < now)
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
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id, includeProperties: "Service.Company,User", tracked: true);
        if (appointment == null)
        {
            return NotFound();
        }
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (appointment.Service.Company.OwnerId != userId)
            {
                return NotFound();
            }
        }
        var now = DateTime.UtcNow;
        var appointmentDateTime = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second);
        if (appointmentDateTime < now)
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
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id, includeProperties: "Service.Company,User", tracked: true);
        if (appointment == null)
        {
            return NotFound();
        }
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (appointment.Service.Company.OwnerId != userId)
            {
                return NotFound();
            }
        }
        if (!appointment.IsCancelationAvailable())
        {
            TempData["error"] = "Appointment already passed or it is too late to cancel appointment";
            return RedirectToAction(nameof(Details), new { id });
        }


        if (appointment.Service.IsPrepaymentRequired)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if (payment is null)
            {
                return NotFound();
            }
            //TODO: Need to extract refund logic to a separate method and call it here
            if (payment.Status == PaymentStatus.Succeeded)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = payment.PaymentIntentId,
                };
                var service = new RefundService();
                service.Create(options);
                unitOfWork.Payment.UpdateStatus(id, AppointmentStatus.Cancelled, PaymentStatus.Refunded);
                TempData["success"] = "Your appointment has been cancelled and money will be refunded in next few days.";
            }
        }
        else
        {
            appointment.Status = AppointmentStatus.Cancelled;
            TempData["success"] = "Your appointment has been cancelled.";
        }

        // TODO: Send email to user about the cancellation

        unitOfWork.Save();
        return RedirectToAction(nameof(Details), new { id });
    }

    #region APICALLS
    public IActionResult GetCompanyAppointments(int Id)
    {
        List<CompanyAppointments> companyAppointments = [];
        var appointments = unitOfWork.Appointments.GetAll(
            u => u.Service.CompanyId == Id,
            includeProperties: "Service.Company,User").ToList();
        if (User.IsInRole(Role.CompanyManager.ToString()) )
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            appointments = appointments.Where(u => u.Service.Company.OwnerId == userId).ToList();
        }


        foreach (var appointment in appointments)
        {
            var companyAppointment = new CompanyAppointments()
            {
                AppointmentId = appointment.Id,
                CompanyId = appointment.Service.CompanyId,
                Date = appointment.Date,
                Time = appointment.Time,
                AppointmentStatus = appointment.Status,
                ServiceName = appointment.Service.Name,
                UserFirstName = appointment.User.FirstName,
                UserLastName = appointment.User.LastName,
                UserEmail = appointment.User.Email,
                UserPhoneNumber = appointment.User.PhoneNumber,
                IsPrepaymentRequired = appointment.Service.IsPrepaymentRequired,

            };
            companyAppointments.Add(companyAppointment);
        }
        return Json(new { data = companyAppointments });
    }
    #endregion
}
