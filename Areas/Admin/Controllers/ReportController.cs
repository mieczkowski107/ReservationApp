using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
public class ReportController(IUnitOfWork unitOfWork, IReportService reportService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IReportService reportService = reportService;

    [BindProperty]
    public DateOnly? StartRangeDate { get; set; }
    [BindProperty]
    public DateOnly? EndRangeDate { get; set; }

    public IActionResult Index()
    {
        if(!StartRangeDate.HasValue|| !EndRangeDate.HasValue)
        {
            var Raport = new Report();
            return View(Raport);
        }
        else
        {
            var Raport = reportService.GetReport(1, StartRangeDate.Value, EndRangeDate.Value);
            return View(Raport);
        }
        
    }

}
