using CsvHelper;
using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webEnviroment;

    public ReportService(IUnitOfWork unitOfWork, IWebHostEnvironment enviroment)
    {
        _unitOfWork = unitOfWork;
        _webEnviroment = enviroment;
    }

    public Report GetReport(int companyId, DateOnly startDate, DateOnly endDate)
    {
        var filteredAppointments = _unitOfWork.Appointments.GetAll(
            p => p.Service.CompanyId == companyId && p.Date >= startDate && p.Date <= endDate,
            includeProperties: "Service.Company,Review"
        );
        var previousClients = _unitOfWork.Appointments
            .GetAll(p => p.Service.CompanyId == companyId && p.Date < startDate)
            .Select(p => p.UserId)
            .Distinct();

        var uniqueClients = filteredAppointments.Select(p => p.UserId).Distinct();
        var newClients = uniqueClients.Except(previousClients).Count();

        var report = new Report
        {
            CompanyId = companyId,
            StartRangeDate = startDate,
            EndRangeDate = endDate,
            Income = filteredAppointments.Sum(p => p.Service.Price),
            Appointments = filteredAppointments.Count(),
            DeletedAppointments = filteredAppointments.Count(p => p.Status == AppointmentStatus.Cancelled),
            NoShowAppointments = filteredAppointments.Count(p => p.Status == AppointmentStatus.NoShow),
            TotalClients = filteredAppointments.Count() - filteredAppointments.Count(p => p.Status == AppointmentStatus.Cancelled),
            UniqueClients = uniqueClients.Count(),
            NewClients = newClients,
            AvgRating = filteredAppointments.Any() ? (decimal?)filteredAppointments.Average(p => p.Review?.Rating) : null
        };

        return report;
    }

    public string GetReportPath(int companyId)
    {
        string wwwRoot = _webEnviroment.WebRootPath;
        string productPath = @"reports\company-" + companyId;
        string finalPath = Path.Combine(wwwRoot, productPath);
        if(!Directory.Exists(finalPath))
        {
            Directory.CreateDirectory(finalPath);
        }
        return finalPath;
    }

    public void WriteReportToCSV(object obj, string Path)
    {
        IEnumerable<object> objects = [obj];
        using var writer = new StreamWriter(Path);
        using var csv = new CsvWriter(writer, culture: System.Globalization.CultureInfo.InvariantCulture);
        csv.WriteRecords(objects);
    }


}
