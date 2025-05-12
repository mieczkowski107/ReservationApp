using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    public IActionResult Index(int? categoryId)
    {
        var companies = unitOfWork.Companies.GetAll(categoryId.HasValue ? u => u.Categories.Any(c => c.Id == categoryId) : null,
                                                    includeProperties: "Categories").ToList();

        var categories = unitOfWork.Categories.GetAll().ToList();

        var categoryCompanyPair = new CompaniesCategoriesVM
        {
            Category = categoryId.HasValue ? categories.Where(u => u.Id == categoryId.Value).FirstOrDefault() : null,
            Companies = companies,
            Categories = categories.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name
            })
        };

        return View(categoryCompanyPair);
    }

    public IActionResult Details(int companyId)
    {
        var services = unitOfWork.Services.GetAll(s => s.CompanyId == companyId, includeProperties: nameof(Company)).ToList();
        var reviews = unitOfWork.Review.GetAll(filter: r => r.Appointment.Service.CompanyId == companyId,
                                                  includeProperties: "Appointment.Service.Company");
        var ratingAvg = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
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
