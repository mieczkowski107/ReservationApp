using ReservationApp.Utility.Enums;

namespace ReservationApp.Services.Interfaces;

public interface INotificationService
{
    public void CreateNotification(NotificationType type, int appointmentId);
    public  Task SendNotification(int appointmentId);
}
