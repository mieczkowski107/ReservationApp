using Microsoft.EntityFrameworkCore;
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
    public decimal Income { get; set; }
    public int? Appointments { get; set; }
    public int? DeletedAppointments { get; set; }
    public int? NoShowAppointments { get; set; }
    public int? TotalClients { get; set; }
    public int? UniqueClients { get; set; }
    public int? NewClients { get; set; }

    [Column(TypeName ="decimal(18,2)")]
    public decimal? AvgRating { get; set; }
    public string? Note { get; set; }

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
}
