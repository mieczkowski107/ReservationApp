using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public AppointmentVM appointmentVM { get; set; }
    public AppointmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(int? ServiceId)
    {
        if(ServiceId == null)
        {
            return NotFound();
        }
        appointmentVM = new AppointmentVM
        {
            ServiceId = (int)ServiceId,
            UserId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value
        };
        return View(appointmentVM);
    }

    [HttpPost]
    public IActionResult ConfirmChoice(AppointmentVM appointmentVM)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", appointmentVM);
        }
        return View(appointmentVM);
    }

    public IActionResult Confirmation()
    {
       

        return View();
    }

    #region ApiCalls
    public IActionResult GetDateAndHours(int ServiceId)
    {
        if (ServiceId == null)
        {
            return NotFound();
        }

        var service = _unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: "Company");
        if (service == null) { return NotFound(); }

        Dictionary<DateOnly, List<TimeOnly>> availableDatesAndHours = new();
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

            List<TimeOnly> availableHours = new();
            var appointments = _unitOfWork.Appointments.GetAll(u => u.Date == i);

            //Wszystkie możliwe godziny w danym dniu
            for (var j = startTime; j < endTime; j = j.AddMinutes(15))
            {
                availableHours.Add(j);
            }
            // Usunięcie godzin, które są zajęte w danym dniu oraz wyeleminowanie godzin, które nie są 
            // dostępne z powodu trwania usługi
            foreach (var appointment in appointments)
            {
                var duration = appointment.Service.DurationMinutes.TotalMinutes;
                var maxTime = appointment.Time.AddMinutes(duration);
                availableHours.RemoveAll(hour =>
                                            appointment.Time <= hour
                                            && hour < maxTime);
            }
            //TO DO:  Usunięcie godzin, które nie są dostępne z powodu zbyt krótkiego czasu na wykonanie tej konkretnej usługi (ServiceId)
            // Np. Usługa trwa 60 minut, więc nie można wybrać godziny 15:45, bo nie starczy czasu na wykonanie usługi
            // Np. Usługa trwa 30 minut, a 8:15 jest już zajęta więc nie można się wpisać na godzinę 8:00


            availableDatesAndHours.Add(i, availableHours);
        }
        return Json(availableDatesAndHours);
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