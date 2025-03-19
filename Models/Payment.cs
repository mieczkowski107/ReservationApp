using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Identity.Client;
using ReservationApp.Utility.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models;

public class Payment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }

    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public decimal Amount { get; set; }

    [ForeignKey("AppointmentId")]
    [ValidateNever]
    public Appointment? Appointment { get; set; }
}
