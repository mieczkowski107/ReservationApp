using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;

namespace ReservationApp.Services;

public class AppointmentStatusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    //private readonly IUnitOfWork _unitOfWork;
    public AppointmentStatusService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
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
                if (appointment.Status == Utility.AppointmentStatus.Pending)
                {
                    appointment.Status = Utility.AppointmentStatus.Cancelled;
                }
                // By default, if the company does not mark as a no-show, it is considered to be done
                else if (appointment.Status == Utility.AppointmentStatus.Confirmed)
                {
                    appointment.Status = Utility.AppointmentStatus.Completed;
                }
                _unitOfWork.Appointments.Update(appointment);
            }
            //_unitOfWork.Save();
            DateTime dateNow = DateTime.UtcNow;
            foreach (var appointment in _unitOfWork.Appointments.GetAll(u => u.CreatedAt.Date == dateNow.Date))
            {
                if (appointment.Status == Utility.AppointmentStatus.Pending && ((dateNow - appointment.CreatedAt).TotalMinutes > TimeSpan.FromMinutes(15).TotalMinutes))
                {
                    appointment.Status = Utility.AppointmentStatus.Cancelled;
                    _unitOfWork.Appointments.Update(appointment);
                }
            }
            _unitOfWork.Save();
            await Task.Delay((int)TimeSpan.FromMinutes(15).TotalMilliseconds, stoppingToken);
        }
    }
}

