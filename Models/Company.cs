using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace ReservationApp.Models;

public class Company
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }
    [Required]
    [MaxLength(255)]    
    public string? Address { get; set; }
    [Required]
    [MaxLength(255)]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid city name. Only letters and spaces are allowed.")]
    public string? City { get; set; }
    [Required]
    [MaxLength(255)]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid city name. Only letters and spaces are allowed.")]
    public string? State { get; set; }
    [Required]
    [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Invalid ZIP code format. Expected format: XX-XXX.")]
    public string? Zip { get; set; }
    [Required]
    [Phone]
    [RegularExpression(@"^\d{3}-\d{3}-\d{3}$", ErrorMessage = "Invalid phone number format. Expected format: XXX-XXX-XXX.")]
    public string? Phone { get; set; }
    [Required]
    [EmailAddress]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string? Description { get; set; }
        
    [ValidateNever]
    public string? ImageUrl { get; set; }

    public Guid? OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    [ValidateNever]
    ApplicationUser? Owner { get; set; }

    public ICollection<Category>? Categories { get; set; }


}
