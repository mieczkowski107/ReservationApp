using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models.ViewModels;

public class ReportVm
{
    public ReportVm(Report report)
    {
        Id = report.Id;
        CompanyName = report.Company?.Name;
        CreatedAt = report.CreatedAt;
        StartRangeDate = report.StartRangeDate;
        EndRangeDate = report.EndRangeDate;
        Income = report.Income;
        Appointments = report.Appointments;
        DeletedAppointments = report.DeletedAppointments;
        NoShowAppointments = report.NoShowAppointments;
        TotalClients = report.TotalClients;
        UniqueClients = report.UniqueClients;
        NewClients = report.NewClients;
        AvgRating = report.AvgRating;
    }
    public int Id { get; set; }
    [Required]
    public string? CompanyName{ get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public DateOnly StartRangeDate { get; set; }
    [Required]
    public DateOnly EndRangeDate { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Income { get; set; }
    public int? Appointments { get; set; }
    public int? DeletedAppointments { get; set; }
    public int? NoShowAppointments { get; set; }
    public int? TotalClients { get; set; }
    public int? UniqueClients { get; set; }
    public int? NewClients { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AvgRating { get; set; }
    public string? ReportUrl { get; set; }

    
}
