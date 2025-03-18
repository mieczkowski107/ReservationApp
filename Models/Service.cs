using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ReservationApp.Models
{
    public class Service
    {
       
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public TimeSpan DurationMinutes { get; set; }
        [Required]
        [DisplayName("Prepayment required")]
        public bool IsPrepaymentRequired { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [ValidateNever]
        [AllowNull]
        public Company? Company { get; set; }

    }
}
