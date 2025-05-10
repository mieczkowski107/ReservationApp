using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class AppointmentCleanupService(IUnitOfWork unitOfWork) : IAppointmentCleanupService
{
    public void ProcessPastAppointments()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        var pastAppointments = unitOfWork.Appointments.GetAll(a => a.Date < today);

        foreach (var appointment in pastAppointments)
        {
            if (appointment.Status == AppointmentStatus.Pending)
            {
                appointment.Status = AppointmentStatus.Cancelled;
            }
            else if (appointment.Status == AppointmentStatus.Confirmed)
            {
                appointment.Status = AppointmentStatus.Completed;
            }

            unitOfWork.Appointments.Update(appointment);
        }

        unitOfWork.Save();
    }

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
