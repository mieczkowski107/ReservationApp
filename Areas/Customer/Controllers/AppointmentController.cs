using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class AppointmentController(IUnitOfWork unitOfWork, INotificationService notificationService, IPaymentService paymentService)
    : Controller
{
    private AppointmentVM? AppointmentVm { get; set; }

    public IActionResult Index(int? ServiceId)
    {
        if (!ServiceId.HasValue)
        {
            return NotFound();
        }
        AppointmentVm = new AppointmentVM
        {
            ServiceId = (int)ServiceId,
        };
        return View(AppointmentVm);
    }

    public IActionResult UserAppointments()
    {
        return View();
    }

    public IActionResult Confirmation(int id)
    {
        var userId = UserService.GetUserId(User).ToString();
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId,
                                                      includeProperties: "Service.Company",
                                                      tracked: true);
        if (appointment == null || appointment.Service != null)
        {
            return NotFound();
        }
        if (appointment!.Service!.IsPrepaymentRequired)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if (payment is null)
            {
                var newPayment = new Payment
                {
                    AppointmentId = appointment.Id,
                    Amount = appointment.Service.Price,
                    Status = PaymentStatus.Pending,
                };
                unitOfWork.Payment.Add(newPayment);
                unitOfWork.Save();
            }
        }
        return View(appointment);
    }

    [HttpPost]
    public IActionResult ConfirmChoice(AppointmentVM appointmentVm)
    {
        if (!ModelState.IsValid || !appointmentVm.IsValid())
        {
            TempData["error"] = "Invalid date or time. Please try again.";
            return RedirectToAction("Index", appointmentVm);
        }

        var availableHours = GetAvailableHours(appointmentVm.ServiceId);

        if (!availableHours[appointmentVm.SelectedDate].Contains(appointmentVm.SelectedTime))
        {
            TempData["error"] = "The selected time is not available. Please choose another one.";
            return RedirectToAction(nameof(Index), new { appointmentVm.ServiceId });
        }

        var appointment = new Appointment
        {
            Date = appointmentVm.SelectedDate,
            Time = appointmentVm.SelectedTime,
            ServiceId = appointmentVm.ServiceId,
            UserId = UserService.GetUserId(User).ToString()
        };
        unitOfWork.Appointments.Add(appointment);
        unitOfWork.Save();

        notificationService.CreateNotification(NotificationType.Confirmation, appointment.Id);
        BackgroundJob.Enqueue(() => notificationService.SendNotification(appointment.Id));

        var jobId = BackgroundJob.Schedule(() => notificationService.CreateNotification(NotificationType.Reminder,appointment.Id), new DateTime(appointment.Date, appointment.Time).AddHours(-24));
        BackgroundJob.ContinueJobWith(jobId, () => notificationService.SendNotification(appointment.Id));

        return RedirectToAction(nameof(Confirmation), new { appointment.Id });
    }

    public IActionResult Details(int id)
    {
        var userId = UserService.GetUserId(User).ToString();
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId,
                                                      includeProperties: "Service.Company",
                                                      tracked: true);
        if (appointment is null)
        {
            return NotFound();
        }
        if (appointment.Status == AppointmentStatus.Pending && !appointment.Service!.IsPrepaymentRequired)
        {
            TempData["success"] = "Your appointment has been confirmed!";
            appointment.Status = AppointmentStatus.Confirmed;
        }
        if (appointment.Status == AppointmentStatus.Pending && appointment.Service.IsPrepaymentRequired)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == appointment.Id);
            if(payment == null)
            {
                return NotFound();
            }
            if (paymentService.IsPaid(payment, out Session session))
            {
                TempData["success"] = "Your appointment has been confirmed!";
                unitOfWork.Payment.UpdateStripePaymentID(appointment.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Payment.UpdateStatus(appointment.Id, AppointmentStatus.Confirmed, PaymentStatus.Succeeded);
            }
        }
        ViewBag.IsReviewed = unitOfWork.Review.Get(u => u.AppointmentId == id) != null;
        unitOfWork.Save();

        return View(appointment);
    }
    public IActionResult Cancel(int id)
    {
        var userId = UserService.GetUserId(User).ToString();
        var appointment = unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId, includeProperties: "Service", tracked: true);
        if (appointment is null)
        {
            return NotFound();
        }

        if (!appointment.IsCancelationAvailable())
        {
            TempData["error"] = "Appointment already passed or it is too late to cancel appointment";
            return RedirectToAction(nameof(UserAppointments));
        }

        if (appointment.Service!.IsPrepaymentRequired)
        {
            var payment = unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if(payment is null)
            {
                return NotFound();
            }
            if (payment.Status == PaymentStatus.Succeeded)
            {
                return RedirectToAction("AppointmentRefund", "Payment", new { Area = "Customer", appointmentId = id });
            }
        }
        else
        {
            appointment.Status = AppointmentStatus.Cancelled;
            TempData["success"] = "Your appointment has been cancelled.";
        }
        notificationService.CreateNotification(NotificationType.Cancellation, appointment.Id);
        unitOfWork.Appointments.Update(appointment);
        unitOfWork.Save();
        return RedirectToAction(nameof(UserAppointments));

    }

    #region APICALLS

    [HttpGet]
    public IActionResult GetDateAndHours(int? ServiceId)
    {
        if (ServiceId == null)
        {
            return NotFound();
        }

        var availableDatesAndHours = GetAvailableHours(ServiceId.Value);
        return Json(availableDatesAndHours);
    }

    [HttpGet]
    public IActionResult GetUserAppointments()
    {
        if (UserService.IsCustomer(User)) 
        {
            var userId = UserService.GetUserId(User).ToString();
            var appointments = unitOfWork.Appointments.GetAll(u => u.UserId == userId && u.Status != AppointmentStatus.Cancelled, includeProperties: "Service.Company").ToList();
            return Json(new { data = appointments });
        }
        else
        {
            return Json(new { });
        }
    }

    #endregion

    private Dictionary<DateOnly, List<TimeOnly>> GetAvailableHours(int ServiceId)
    {
        var availableDatesAndHours = new Dictionary<DateOnly, List<TimeOnly>>();

        var service = unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: nameof(Company));
        if (service == null) 
        {
            return availableDatesAndHours;
        }

        // Ustalenie pierwszej i ostatniej daty, które można wybrać
        var firstDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)); // Nie można wybrać dzisiejszej daty
        var lastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)); // Nie można wybrać daty późniejszej niż 30 dni od dzisiejszej daty

        // Ustalenie godzin pracy firmy
        var startTime = new TimeOnly(8, 0, 0); // In the future, this should be fetched from the database based on the company working hours 
        var endTime = new TimeOnly(16, 0, 0); // In the future, this should be fetched from the database based on the company working hours


        for (var i = firstDate; i <= lastDate; i = i.AddDays(1))
        {
            if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            var availableHours = new List<TimeOnly>();
            var appointments = unitOfWork.Appointments.GetAll(u => u.Date == i && u.Service.CompanyId == service.CompanyId, includeProperties: "Service");

            //Wszystkie możliwe godziny w danym dniu
            for (var j = startTime; j < endTime; j = j.AddMinutes(15))
            {
                availableHours.Add(j);
            }
            var busyHours = new List<TimeOnly>(availableHours);
            // Usunięcie godzin, które są zajęte w danym dniu
            // oraz wyeleminowanie godzin, które nie są 
            // dostępne z powodu trwania usługi
            foreach (var appointment in appointments)
            {
                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    continue;
                }
                var duration = appointment.Service!.DurationMinutes.TotalMinutes;
                var maxTime = appointment.Time.AddMinutes(duration);
                availableHours.RemoveAll(hour =>
                                            appointment.Time <= hour
                                            && hour < maxTime);
            }

            //Usunięcie godzin, które nie są dostępne z powodu zbyt krótkiego czasu na wykonanie tej konkretnej usługi (ServiceId)
            // Np. Usługa trwa 60 minut, więc nie można wybrać godziny 15:45, bo nie starczy czasu na wykonanie usługi
            // Np. Usługa trwa 30 minut, a 8:15 jest już zajęta więc nie można się wpisać na godzinę 8:00
            busyHours = busyHours.Except(availableHours).ToList();
            int intervalCount = ((int)service.DurationMinutes.TotalMinutes) / 15;

            for (int m = 0; m < availableHours.Count; m++)
            {
                var available = availableHours[m];
                for (int k = 0; k < intervalCount; k++)
                {
                    if (busyHours.Contains(available.AddMinutes(15)))
                    {
                        availableHours.RemoveAt(m);
                        m--; 
                        break;
                    }
                }
            }
            availableDatesAndHours.Add(i, availableHours);
        }
        return availableDatesAndHours;
    }
}


//Założenia:
/*
    Każda firma pracuje od poniedziałku do piątku w godzinach 8:00-16:00
    W firmie jest tylko jeden pracownik, który wykonuje usługi (Uproszczenie)


    Wizyty:
    Klient może wybrać dowolny dzień i godzinę w zakresie godzin pracy firmy
    Klient nie może wybrać godziny, która już jest zajęta
    Klient nie może wybrać daty dzisiejszej oraz jutrzejrzej
    Klient nie może wybrać godziny, która jest późniejsza niż 30 dni od dzisiejszej daty
    Możliwe godziny do wybrania są co 15 minut zaczynając od 8:00, 8:15, 8:30 itd.  aż do 15:45
    Usługa, blokuje wszystkie godziny w swoim czasie trwania
 */