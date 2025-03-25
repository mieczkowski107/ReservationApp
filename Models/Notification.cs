using ReservationApp.Utility.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Models;

public class Notification
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(1000)]
    public string? Message { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public NotificationStatus Status { get;set; }
    [Required]
    public NotificationType Type { get; set; }

    [Required]
    public string userEmail { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    public virtual Appointment? Appointment { get; set; }

}
