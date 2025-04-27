using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    public IActionResult Index()
    {
        var companies = unitOfWork.Companies.GetAll(includeProperties: nameof(Category)).ToList();
        var categories = unitOfWork.Categories.GetAll().ToList();
        var companiesCategories = new CompaniesCategoriesVM
        {
            Companies = companies,
            Categories = categories
        };
        return View(companiesCategories);
    }


    public IActionResult Details(int companyId)
    {
        var services = unitOfWork.Services.GetAll(s => s.CompanyId == companyId, includeProperties: nameof(Company)).ToList();
        var reviews = unitOfWork.Review.GetAll(filter: r => r.Appointment.Service.CompanyId == companyId,
                                                  includeProperties: "Appointment.Service.Company");
        var ratingAvg = reviews.Any() ?  reviews.Average(r => r.Rating) : 0;
        var quantity = reviews.Count();

        if (!services.Any())
        {
            TempData["error"] = "Something went wrong.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Rating = ratingAvg.ToString("F2");
        ViewBag.Quantity = quantity;

        return View(services);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
