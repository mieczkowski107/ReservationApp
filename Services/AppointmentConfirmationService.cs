using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class AppointmentConfirmationService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>(); 
            DateTime dateNow = DateTime.UtcNow;
            foreach (var appointment in _unitOfWork.Appointments.GetAll(u => u.CreatedAt.Date == dateNow.Date))
            {
                if (appointment.Status == AppointmentStatus.Pending && ((dateNow - appointment.CreatedAt).TotalMinutes > TimeSpan.FromMinutes(15).TotalMinutes))
                {
                    appointment.Status = AppointmentStatus.Cancelled;
                    _unitOfWork.Appointments.Remove(appointment);
                }
            }
            _unitOfWork.Save();
            await Task.Delay((int)TimeSpan.FromMinutes(15).TotalMilliseconds, stoppingToken);
        }
    }
}

