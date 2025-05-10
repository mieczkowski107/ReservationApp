using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class AppointmentCleanupService(IUnitOfWork unitOfWork) : IAppointmentCleanupService
{
    public void RemoveOutdatedPendingAppointments(int thresholdMinutes)
    {
        var dateNow = DateTime.UtcNow;
        foreach (var appointment in unitOfWork.Appointments.GetAll(u => u.Status == AppointmentStatus.Pending))
        {
            var isOverThreshold = ((dateNow - appointment.CreatedAt).TotalMinutes > thresholdMinutes);
            if (isOverThreshold)
            {
                unitOfWork.Appointments.Remove(appointment);
            }
        }
        unitOfWork.Save();
    }
}
