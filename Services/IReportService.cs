using ReservationApp.Models;

namespace ReservationApp.Services;

public interface IReportService
{
    Report GetReport(int companyId,DateOnly startDate, DateOnly endDate);
}
