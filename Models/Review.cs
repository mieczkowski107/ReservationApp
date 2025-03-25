using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models;

public class Review
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string? UserId { get; set; }
    [Required]
    [MaxLength(1000)]
    public string? Content { get; set; }
    [Required]
    [Range(1,5)]
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [ForeignKey("ServiceId")]
    [ValidateNever]
    public virtual Service? Service { get; set; }
    [ForeignKey("UserId")]
    [ValidateNever]
    public virtual ApplicationUser? User { get; set; }
}
