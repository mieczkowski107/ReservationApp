using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AppointmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index(int? ServiceId)
    {
        if (ServiceId == null)
        {
            return NotFound();
        }

        var service = _unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: "Company");
        if (service == null) { return NotFound(); }

        Dictionary<DateOnly, List<TimeOnly>> availableDatesAndHours = new();
        // Ustalenie pierwszej i ostatniej daty, które można wybrać
        DateOnly firstDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)); // Nie można wybrać dzisiejszej daty
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
            // Usunięcie godzin, które nie są dostępne z powodu zbyt krótkiego czasu na wykonanie tej konkretnej usługi (ServiceId)
            for (int j = availableHours.Count - 1; j > 0; j--)
            {
                var difference = availableHours[j] - availableHours[j - 1];
                if (difference.TotalMinutes < service!.DurationMinutes.TotalMinutes)
                {
                    availableHours.RemoveAt(j);
                }
            }
            availableDatesAndHours.Add(i, availableHours);
        }

        return View(availableDatesAndHours);
    }
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