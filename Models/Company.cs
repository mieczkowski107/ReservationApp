using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models
{
    public class Company
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]    
        public string Address { get; set; }
        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid city name. Only letters and spaces are allowed.")]
        public string City { get; set; }
        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid city name. Only letters and spaces are allowed.")]
        public string State { get; set; }
        [Required]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Invalid ZIP code format. Expected format: XX-XXX.")]
        public string Zip { get; set; }
        [Required]
        [Phone] 
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

    }
}
