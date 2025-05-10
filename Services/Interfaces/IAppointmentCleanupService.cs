namespace ReservationApp.Services.Interfaces;

public interface IAppointmentCleanupService
{
    void RemoveOutdatedPendingAppointments(int thresholdMinutes);
}
