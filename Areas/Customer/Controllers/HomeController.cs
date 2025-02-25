using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.CodeAnalysis;
using Mono.TextTemplating;
using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        
    }

    public IActionResult Index()
    {
        var companies = _unitOfWork.Companies.GetAll(includeProperties: nameof(Category)).ToList();
        var categories = _unitOfWork.Categories.GetAll().ToList();
        var companiesCategories = new CompaniesCategoriesVM
        {
            Companies = companies,
            Categories = categories
        };
        return View(companiesCategories);
    }

  
    public IActionResult Details(int companyId)
    {
        var services = _unitOfWork.Services.GetAll(s => s.CompanyId == companyId, includeProperties: nameof(Company)).ToList();
        if(services == null || services.Count() == 0)
        {
            TempData["error"] = "Something went wrong.";
            return RedirectToAction(nameof(Index));
        }
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
