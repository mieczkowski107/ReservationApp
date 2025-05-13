
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ReservationApp.Models.ViewModels;

public class CompanyVM
{
    public Company? Company { get; set; }
    [ValidateNever]

    public IEnumerable<SelectListItem> CategoryList { get; set; }
    [AllowNull]
    public List<int>? CategoriesId { get; set; }
}
