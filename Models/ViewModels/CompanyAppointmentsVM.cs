using ReservationApp.Utility.Enums;

namespace ReservationApp.Models.ViewModels;

public class CompanyAppointmentsVM
{
    private CompanyAppointmentsVM(Appointment appointment)
    {
        AppointmentId = appointment.Id;
        CompanyId = appointment.Service!.CompanyId;
        CompanyName = appointment.Service?.Company?.Name;
        Date = appointment.Date;
        Time = appointment.Time;
        AppointmentStatus = appointment.Status;
        DurationMinutes = appointment.Service!.DurationMinutes;
        Price = appointment.Service.Price;
        ServiceName = appointment.Service.Name;
        UserFirstName = appointment.User!.FirstName;
        UserLastName = appointment.User.LastName;
        UserEmail = appointment.User.Email;
        UserPhoneNumber = appointment.User.PhoneNumber;
        IsPrepaymentRequired = appointment.Service.IsPrepaymentRequired;
    }
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

    public static CompanyAppointmentsVM MapFromAppointment(Appointment appointment)
    {
        return new CompanyAppointmentsVM(appointment);
    }
}
