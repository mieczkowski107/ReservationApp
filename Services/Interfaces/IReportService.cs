using ReservationApp.Models;

namespace ReservationApp.Services.Interfaces;

public interface IReportService
{
    Report GetReport(int companyId,DateOnly startDate, DateOnly endDate);
    public string GetReportPath(int companyId);
    public void WriteReportToCSV(object obj, string Path);
}
