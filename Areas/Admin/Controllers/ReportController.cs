using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;

namespace ReservationApp.Areas.Admin.Controllers;

public class ReportController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        return View();
    }

   
}
