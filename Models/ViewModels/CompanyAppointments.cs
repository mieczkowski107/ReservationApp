using ReservationApp.Utility.Enums;

namespace ReservationApp.Models.ViewModels;

public class CompanyAppointments
{
    public int AppointmentId { get; set; }
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? ServiceName { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public TimeSpan DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhoneNumber { get; set; }
    public bool IsPrepaymentRequired { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? PaymentIntentId { get; set; }

}
