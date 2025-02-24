
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReservationApp.Models.ViewModels
{
    public class CompanyVM
    {
        public Company? Company { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
