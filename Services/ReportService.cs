using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
            AvgRating = (decimal)filteredAppointments.Average(p =>
            {
                return p.Review?.Rating;
            })
        };

        return report;
    }


}
