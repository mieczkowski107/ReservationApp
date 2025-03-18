using ReservationApp.Utility.Enums;

namespace ReservationApp.Models.ViewModels;

public class CompanyAppointments
{
    public int Id { get; set; }
    public string? ServiceName { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhoneNumber { get; set; }
    public bool IsPrepaymentRequired { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? PaymentIntentId { get; set; }




}
