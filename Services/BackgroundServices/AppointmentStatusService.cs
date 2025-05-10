using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services.BackgroundServices;

public class AppointmentStatusService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var processPastAppointments = scope.ServiceProvider.GetRequiredService<IAppointmentCleanupService>();
            processPastAppointments.ProcessPastAppointments();

            await Task.Delay((int)TimeSpan.FromHours(1).TotalMilliseconds, stoppingToken);
        }
    }
}

