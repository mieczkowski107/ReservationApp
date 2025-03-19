using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Utility.Enums;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AppointmentVM AppointmentVM { get; set; }
    public AppointmentController(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(int? ServiceId)
    {
        if (ServiceId == null)
        {
            return NotFound();
        }
        AppointmentVM = new AppointmentVM
        {
            ServiceId = (int)ServiceId,
        };
        return View(AppointmentVM);
    }

    public IActionResult UserAppointments()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ConfirmChoice(AppointmentVM appointmentVM)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", appointmentVM);
        }

        if (!appointmentVM.IsValid())
        {
            TempData["error"] = "Invalid date or time. Please try again.";
            return RedirectToAction("Index", appointmentVM);
        }

        var availableHours = new Dictionary<DateOnly, List<TimeOnly>>();

        #region JsonDeserialize
        var json = GetDateAndHours(appointmentVM.ServiceId).ToJson();
        var jsonNode = JsonNode.Parse(json);
        var valueNode = jsonNode?["Value"];

        if (valueNode != null)
        {
            var tempDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(valueNode.ToJsonString());

            if (tempDict != null)
            {
                availableHours = tempDict.ToDictionary(
                    kvp => DateOnly.Parse(kvp.Key),
                    kvp => kvp.Value.Select(time => TimeOnly.Parse(time)).ToList()
                );
            }
        }
        #endregion

        if (!availableHours[appointmentVM.SelectedDate].Contains(appointmentVM.SelectedTime))
        {
            TempData["error"] = "The selected time is not available. Please choose another one.";
            return RedirectToAction(nameof(Index), new { appointmentVM.ServiceId });
        }

        var appointment = new Appointment
        {
            Date = appointmentVM.SelectedDate,
            Time = appointmentVM.SelectedTime,
            ServiceId = appointmentVM.ServiceId,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };


        _unitOfWork.Appointments.Add(appointment);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Confirmation), new { appointment.Id });
    }

    public IActionResult Confirmation(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var appointment = _unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId, includeProperties: "Service.Company", tracked: true);
        if (appointment == null)
        {
            return NotFound();
        }
        if (appointment.Service.IsPrepaymentRequired)
        {
            var payment = _unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if (payment is null)
            {
                var newPayment = new Payment
                {
                    AppointmentId = appointment.Id,
                    Amount = appointment.Service.Price,
                    Status = PaymentStatus.Pending,
                };
                _unitOfWork.Payment.Add(newPayment);
                _unitOfWork.Save();
            }
        }
        return View(appointment);
    }


    public IActionResult Details(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var appointment = _unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId, includeProperties: "Service.Company", tracked: true);
        if (appointment is null)
        {
            return NotFound();
        }
        if (appointment.Status == AppointmentStatus.Pending && !appointment.Service.IsPrepaymentRequired)
        {
            TempData["success"] = "Your appointment has been confirmed!";
            appointment.Status = AppointmentStatus.Confirmed;
        }
        if (appointment.Status == AppointmentStatus.Pending && appointment.Service.IsPrepaymentRequired)
        {
            //TODO: Need to extract logic to a separate method and call it here
            var payment = _unitOfWork.Payment.Get(u => u.AppointmentId == appointment.Id);
            var service = new SessionService();
            Session session = service.Get(payment.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                TempData["success"] = "Your appointment has been confirmed!";
                _unitOfWork.Payment.UpdateStripePaymentID(appointment.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Payment.UpdateStatus(appointment.Id, AppointmentStatus.Confirmed, PaymentStatus.Succeeded);
            }
        }

        _unitOfWork.Appointments.Update(appointment);
        _unitOfWork.Save();

        return View(appointment);
    }
    public IActionResult Cancel(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var appointment = _unitOfWork.Appointments.Get(u => u.Id == id && u.UserId == userId, includeProperties: "Service", tracked: true);
        if (appointment is null)
        {
            return NotFound();
        }
        if(appointment.Status == AppointmentStatus.Cancelled)
        {
            TempData["error"] = "Appointment already cancelled";
            return RedirectToAction(nameof(UserAppointments));
        }

        if (!appointment.IsCancelationAvailable())
        {
            TempData["error"] = "Appointment already passed or it is too late to cancel appointment";
            return RedirectToAction(nameof(UserAppointments));
        }

        if (appointment.Service.IsPrepaymentRequired)
        {
            var payment = _unitOfWork.Payment.Get(u => u.AppointmentId == id);
            if(payment is null)
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
                _unitOfWork.Payment.UpdateStatus(id, AppointmentStatus.Cancelled, PaymentStatus.Refunded);
                TempData["success"] = "Your appointment has been cancelled and money will be refunded in next few days.";
            }
        }
        else
        {
            appointment.Status = AppointmentStatus.Cancelled;
            TempData["success"] = "Your appointment has been cancelled.";
        }
        _unitOfWork.Appointments.Update(appointment);
        _unitOfWork.Save();
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

        Dictionary<DateOnly, List<TimeOnly>> availableDatesAndHours = [];

        var service = _unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: nameof(Company));
        if (service == null) { return NotFound(); }


        // Ustalenie pierwszej i ostatniej daty, które można wybrać
        DateOnly firstDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)); // Nie można wybrać dzisiejszej daty
        DateOnly lastDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)); // Nie można wybrać daty późniejszej niż 30 dni od dzisiejszej daty

        // Ustalenie godzin pracy firmy
        TimeOnly startTime = new(8, 0, 0); // In future, this should be fetched from the database based on the company working hours 
        TimeOnly endTime = new(16, 0, 0); // In future, this should be fetched from the database based on the company working hours


        for (var i = firstDate; i <= lastDate; i = i.AddDays(1))
        {
            if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            List<TimeOnly> availableHours = [];
            var appointments = _unitOfWork.Appointments.GetAll(u => u.Date == i && u.Service.CompanyId == service.CompanyId, includeProperties: "Service");

            //Wszystkie możliwe godziny w danym dniu
            for (var j = startTime; j < endTime; j = j.AddMinutes(15))
            {
                availableHours.Add(j);
            }
            List<TimeOnly> busyHours = new(availableHours);
            // Usunięcie godzin, które są zajęte w danym dniu
            // oraz wyeleminowanie godzin, które nie są 
            // dostępne z powodu trwania usługi
            foreach (var appointment in appointments)
            {
                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    continue;
                }
                var duration = appointment.Service.DurationMinutes.TotalMinutes;
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
                        m--; // Decrement the index since we've removed an item
                        break;
                    }
                }
            }

            availableDatesAndHours.Add(i, availableHours);
        }
        return Json(availableDatesAndHours);
    }

    [HttpGet]
    public IActionResult GetUserAppointments()
    {
        if (User.IsInRole(Role.Customer.ToString())) //TODO: DO POPRAWY ten if
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = _unitOfWork.Appointments.GetAll(u => u.UserId == userId, includeProperties: "Service.Company").ToList();
            return Json(new { data = appointments });
        }
        else
        {
            return Json(new { });
        }
    }

    #endregion

}


//Założenia:
/*
    Każda firma pracuje od poniedziałku do piątku w godzinach 8:00-16:00
    W firmie jest tylko jeden pracownik, który wykonuje usługi (Uproszczenie)
    Klient może wybrać dowolny dzień i godzinę w zakresie godzin pracy firmy
    Klient nie może wybrać godziny, która już jest zajęta
    Klient nie może wybrać daty dzisiejszej oraz jutrzejrzej
    Klient nie może wybrać godziny, która jest późniejsza niż 30 dni od dzisiejszej daty
    Możliwe godziny do wybrania są co 15 minut zaczynając od 8:00, 8:15, 8:30 itd.  aż do 15:45
    Usługa, blokuje wszystkie godziny w swoim czasie trwania
 */