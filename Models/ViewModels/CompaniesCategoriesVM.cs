using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReservationApp.Models.ViewModels;

public class CompaniesCategoriesVM
{
    public Category? Category { get; set; }
    public  List<Company> Companies { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
}
