using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models;

public class Review
{
    public int Id { get; set; }
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [MaxLength(1000)]
    [Display(Name = "Review")]
    public string? Content { get; set; }
    [Required]
    [Range(1,5)]
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [ForeignKey("AppointmentId")]
    [ValidateNever]
    public virtual Appointment? Appointment { get; set; }
  
}
