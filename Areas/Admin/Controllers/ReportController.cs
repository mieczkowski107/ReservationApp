using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
public class ReportController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [BindProperty]
    public DateOnly StartRangeDate { get; set; }
    [BindProperty]
    public DateOnly EndRangeDate { get; set; }

    public IActionResult Index()
    {
        var x = StartRangeDate;
        var y = EndRangeDate;
        return View();
    }
   
}
