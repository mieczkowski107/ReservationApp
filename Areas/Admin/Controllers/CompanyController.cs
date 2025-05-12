using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using NuGet.Packaging;
using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using System.Linq;
using System.Security.Claims;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class CompanyController(ApplicationDbContext db, IUnitOfWork _unitOfWork, IImageService _imageService)
    : Controller
{

    public IActionResult Index()
    {
        var isAdmin = UserService.IsAdmin(User);
        var userId = UserService.GetUserId(User);
        var companies = _unitOfWork.Companies.GetAll(includeProperties: "Categories").ToList();
        if (!isAdmin && !companies.Any(u => u.OwnerId == userId))
        {
            companies.Clear();
        }
        if (UserService.IsCompanyManager(User))
        {
            companies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId, includeProperties: "Categories").ToList();
        }

        return View(companies);
    }

    public IActionResult Create()
    {
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            var userCompanies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId);
            if (userCompanies.Any())
            {
                return Forbid();
            }
        }
        var companyVm = new CompanyVM()
        {
            Company = new Company(),
            CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            })
        };
        return View(companyVm);
    }

    [HttpPost]
    [ActionName("Create")]
    public IActionResult CreatePost(CompanyVM companyVm, IFormFile? file)
    {
        var userId = UserService.GetUserId(User);
        if (!UserService.IsAdmin(User))
        {
            var userCompany = _unitOfWork.Companies.Get(u => u.OwnerId == userId);
            if (userCompany != null)
            {
                return Forbid();
            }
        }

        var categoriesForCompany = companyVm.CategoriesId != null ?
                                  _unitOfWork.Categories.GetAll(c => companyVm.CategoriesId.Contains(c.Id), tracked: true).ToList()
                                  : new List<Category>();

        if (!categoriesForCompany.Any())
        {
            ModelState.TryAddModelError("No required field: Categories", "No required field: Categories");
        }

        if (ModelState.IsValid)
        {
            companyVm.Company.OwnerId = userId;
            companyVm.Company.Categories = categoriesForCompany.ToList();
            _imageService.ImageUpload(companyVm, file);
            _unitOfWork.Companies.Add(companyVm.Company!);
            TempData["success"] = "Company added successfully!";
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        return RedirectToAction(nameof(Create));
    }


    public IActionResult Edit(int id)
    {
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            var userCompany = _unitOfWork.Companies.Get(u => u.OwnerId == userId && u.Id == id);
            if (userCompany == null)
            {
                return Forbid();
            }
        }
        var company = _unitOfWork.Companies.Get(u => u.Id == id, includeProperties: "Categories");
        var companyVm = new CompanyVM()
        {
            Company = company,
            CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            })
        };
        return View(companyVm);
    }

    [HttpPost]
    public IActionResult Edit(CompanyVM companyVm, IFormFile? file)
    {
        var categoriesForCompany = _unitOfWork.Categories.GetAll(u => companyVm.CategoriesId.Contains(u.Id), tracked: true);
        if (ModelState.IsValid)
        {
            var userId = UserService.GetUserId(User);
            var userCompany = _unitOfWork.Companies.Get(u => u.OwnerId == userId
                                                             && u.Id == companyVm.Company.Id, includeProperties: "Categories", tracked: true);
            if (userCompany == null)
            {
                return Forbid();
            }
            if (categoriesForCompany != null)
            {
                userCompany.Categories = categoriesForCompany.ToList();
            }
            _imageService.ImageUpload(companyVm, file);
            _unitOfWork.Save();
            TempData["success"] = "Company updated successfully!";
            return RedirectToAction("Index");
        }
        return RedirectToAction(nameof(Edit), new { companyVm?.Company?.Id });
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var isAdmin = UserService.IsAdmin(User);
        var userId = UserService.GetUserId(User);
        var userCompany = _unitOfWork.Companies.Get(u => u.Id == id && (u.OwnerId == userId || isAdmin), tracked: true);

        if (userCompany == null)
        {
            return NotFound();
        }
        return View(userCompany);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var isAdmin = UserService.IsAdmin(User);
        var userId = UserService.GetUserId(User);
        var userCompany = _unitOfWork.Companies.Get(u => u.Id == id && (u.OwnerId == userId || isAdmin), tracked: true);

        if (userCompany == null)
        {
            TempData["error"] = "Error while deleting company";
            return RedirectToAction(nameof(Index));
        }
        _imageService.DeleteImage(userCompany!.ImageUrl!);
        _unitOfWork.Companies.Remove(userCompany);
        _unitOfWork.Save();        

        TempData["success"] = "Company successfully deleted";
        return RedirectToAction(nameof(Index));
    }


    #region APICALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        if (UserService.IsAdmin(User))
        {
            var allObj = _unitOfWork.Companies.GetAll();
            return Json(new { data = allObj });
        }
        else
        {
            var userId = UserService.GetUserId(User);
            var allObj = _unitOfWork.Companies.GetAll(c => c.OwnerId == userId);
            return Json(new { data = allObj });
        }

    }

    #endregion
}
