using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services.BackgroundServices;

public class AppointmentConfirmationService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private int _cancellationThreshold = 15;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var cleanupService = scope.ServiceProvider.GetRequiredService<IAppointmentCleanupService>();
            cleanupService.RemoveOutdatedPendingAppointments(_cancellationThreshold);
            await Task.Delay((int)TimeSpan.FromMinutes(10).TotalMilliseconds, stoppingToken);
        }
    }
}

