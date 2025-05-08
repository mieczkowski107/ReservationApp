using CsvHelper;
using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using System.Linq;

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
            p => p.Service!.CompanyId == companyId
            && p.Date >= startDate
            && p.Date <= endDate,
            includeProperties: "Service.Company,Review"
        );
        var previousAppointments = _unitOfWork.Appointments
            .GetAll(p => p.Service!.CompanyId == companyId
            && p.Date < startDate, includeProperties: nameof(Service));

        var report = Report.CreateReport(companyId, startDate, endDate, filteredAppointments, previousAppointments);

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
