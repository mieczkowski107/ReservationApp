namespace ReservationApp.Utility;

public enum AppointmentStatus
{
    Pending, // Created, not confirmed or awaiting payment
    Paid,    // Paid, confirmed
    Confirmed, // Confirmed
    Completed, // Has taken place and completed
    NoShow,    // Did not show up
    Cancelled  // Cancelled
}
