namespace ReservationApp.Models.ViewModels;

public class AppointmentVM
{
    public int ServiceId { get; set; }
    public DateOnly SelectedDate { get; set; }
    public TimeOnly SelectedTime { get; set; }

    public bool IsValid ()
    {
        if(SelectedDate == null || SelectedTime == null)
        {
            return false;
        }
        if (SelectedDate < DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)) || SelectedDate > DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
        {
            return false;
        }
        if(SelectedDate.DayOfWeek == DayOfWeek.Saturday || SelectedDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }
        if (SelectedTime < new TimeOnly(8, 0, 0) || SelectedTime > new TimeOnly(16, 0, 0))
        {
            return false;
        }

        return true;
    }
}
