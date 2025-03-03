namespace ReservationApp.Models.ViewModels;

public class AppointmentVM
{
    public int ServiceId { get; set; }
    public DateOnly SelectedDate { get; set; }
    public TimeOnly SelectedTime { get; set; }
    public string UserId { get; set; }
}
