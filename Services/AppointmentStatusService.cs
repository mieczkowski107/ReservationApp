using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class AppointmentStatusService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var pastAppointments = _unitOfWork.Appointments.GetAll(a => a.Date < today, tracked: false);
            foreach (var appointment in pastAppointments)
            {
                // If the appointment is not confirmed or paid, it is considered to be cancelled
                if (appointment.Status == AppointmentStatus.Pending)
                {
                    appointment.Status = AppointmentStatus.Cancelled;
                }
                // By default, if the company does not mark as a no-show, it is considered to be done
                else if (appointment.Status == AppointmentStatus.Confirmed)
                {
                    appointment.Status = AppointmentStatus.Completed;

                }
                _unitOfWork.Appointments.Update(appointment);
                _unitOfWork.Save();
            }
            await Task.Delay((int)TimeSpan.FromMinutes(60).TotalMilliseconds, stoppingToken);
        }
    }
}

