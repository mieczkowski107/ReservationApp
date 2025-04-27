using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Models.ViewModels;

public class ReportFormVM
{
    [ValidateNever]
    public IEnumerable<SelectListItem>? CompanyList { get; set; }
    [Required]
    public int CompanyId { get; set; }
    [Required]
    [DisplayName("Start Date")]
    public DateOnly StartRangeDate { get; set; }
    [Required]
    [DisplayName("End Date")]
    public DateOnly EndRangeDate { get; set; }

}
