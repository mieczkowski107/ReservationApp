using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Utility.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models;

public class Report
{
    public int Id { get; set; }
    [Required]
    public int CompanyId { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public DateOnly StartRangeDate { get; set; }
    [Required]
    public DateOnly EndRangeDate { get; set; }
    [Required]
    [Column(TypeName ="decimal(18,2)")]
    public decimal? Income { get; set; }
    public int? Appointments { get; set; }
    public int? DeletedAppointments { get; set; }
    public int? NoShowAppointments { get; set; }
    public int? TotalClients { get; set; }
    public int? UniqueClients { get; set; }
    public int? NewClients { get; set; }

    [Column(TypeName ="decimal(18,2)")]
    public decimal? AvgRating { get; set; }
    public string? ReportUrl { get; set; }

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }

    public static Report CreateReport(
      int companyId,
      DateOnly startDate,
      DateOnly endDate,
      IEnumerable<Appointment> appointmentsInRangeDate,
      IEnumerable<Appointment> appointmentsBefore)
    {
        var inRangeList = appointmentsInRangeDate.ToList();
        var beforeList = appointmentsBefore.ToList();

        var previousClients = beforeList
            .Where(p => p.Service!.CompanyId == companyId && p.Date < startDate)
            .Select(p => p.UserId)
            .Distinct()
            .ToHashSet(); 

  
        var uniqueClients = inRangeList
            .Select(p => p.UserId)
            .Distinct()
            .ToList();

        int newClients = uniqueClients.Count(userId => !previousClients.Contains(userId));


        int totalAppointments = inRangeList.Count;
        int cancelledAppointments = inRangeList.Count(p => p.Status == AppointmentStatus.Cancelled);
        int noShowAppointments = inRangeList.Count(p => p.Status == AppointmentStatus.NoShow);
        int totalClients = totalAppointments - cancelledAppointments;

        decimal? avgRating = inRangeList.Any()
            ? (decimal?)inRangeList.Average(p => p.Review?.Rating)
            : null;

        decimal income = inRangeList.Sum(p => p.Service!.Price);

        return new Report
        {
            CompanyId = companyId,
            StartRangeDate = startDate,
            EndRangeDate = endDate,
            Income = income,
            Appointments = totalAppointments,
            DeletedAppointments = cancelledAppointments,
            NoShowAppointments = noShowAppointments,
            TotalClients = totalClients,
            UniqueClients = uniqueClients.Count,
            NewClients = newClients,
            AvgRating = avgRating
        };
    }


}
