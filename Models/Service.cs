using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApp.Models
{
    public class Service
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public TimeSpan DurationMinutes { get; set; }
        [Required]
        public bool IsPrepaymentRequired { get; set; }
        [Required]
        public int CompanyId { get; set; }
        
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company Company { get; set; }



    }
}
