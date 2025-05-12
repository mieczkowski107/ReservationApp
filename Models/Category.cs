using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid city name. Only letters and spaces are allowed.")]
        [Required]
        public string? Name { get; set; }
        public ICollection<Company>? Companies { get; set; }
    }
}
