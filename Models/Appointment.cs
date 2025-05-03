using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ReservationApp.Utility.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models;
public class Appointment
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? UserId { get; set; }
    [Required]
    public int ServiceId { get; set; }
    [Required]
    [Display(Name = "Appointment Date")]
    public DateOnly Date { get; set; }
    [Required]
    [Display(Name = "Appointment Time")]
    public TimeOnly Time { get; set; }
    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [ForeignKey("UserId")]
    [ValidateNever]
    public ApplicationUser? User { get; set; }
    [ForeignKey("ServiceId")]
    [ValidateNever]
    public Service? Service { get; set; }

    [ValidateNever]
    public virtual Review? Review { get; set; }

    [ValidateNever]
    [NotMapped]
    public DateTime AppointmentDateTime => new(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second);
    public bool IsCancelationAvailable()
    {
        var now = DateTime.UtcNow;
        var appointmentDateTime = new DateTime(Date,Time);
        var diff = appointmentDateTime - now;
        return diff.TotalHours > 25 || Status == AppointmentStatus.Cancelled || Status == AppointmentStatus.Completed || Status == AppointmentStatus.NoShow;
    }
    

}
