using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using System.Security.Claims;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class CompanyController(IUnitOfWork _unitOfWork, IWebHostEnvironment _hostEnvironment, IImageService _imageService)
    : Controller
{

    public IActionResult Index()
    {
        var companies = _unitOfWork.Companies.GetAll(includeProperties: nameof(Category)).ToList();
        if (!UserService.IsAdmin(User) && !companies.Any(u => u.OwnerId == UserService.GetUserId(User)))
        {
            return NotFound();
        }
        if (UserService.IsCompanyManager(User))
        {
            var userId = UserService.GetUserId(User);
            companies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId, includeProperties: "Category").ToList();
        }

        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            var userCompanies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId).Select(c => c.Id);
            if (id.HasValue)
            {
                if (!userCompanies.Contains(id.Value))
                {
                    return Forbid();
                }
            }
            else
            {
                //TODO: Add logic to check if user is owner of company and has access to Create or Edit
                // Probably need to add field in User table to check if user can create company if id is null
                // Or check if user is owner of company and has access to edit
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
        if (id == null || id == 0)
        {
            return View(companyVm);
        }
        else
        {
            companyVm.Company = _unitOfWork.Companies.Get(c => c.Id == id);
            if (companyVm.Company == null)
            {
                return NotFound();
            }
            return View(companyVm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(CompanyVM companyVm, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            _imageService.ImageUpload(companyVm, file);
         
            if (companyVm.Company.OwnerId == null)
            {
                var userId = UserService.GetUserId(User);
                companyVm.Company.OwnerId = userId;
                _unitOfWork.Companies.Add(companyVm.Company!);
                TempData["success"] = "Company added successfully!";
            }
            else
            {
                _unitOfWork.Companies.Update(companyVm.Company);
                TempData["success"] = "Company updated successfully!";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = "Something went wrong!";
            return RedirectToAction(nameof(Upsert), new { companyVm?.Company?.Id });
        }
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            var userCompanies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId).Select(c => c.Id);
            if (!userCompanies.Contains(id.Value))
            {
                return Forbid();
            }
        }
        var company = _unitOfWork.Companies.Get(c => c.Id == id, includeProperties: nameof(Category));
        if (company == null)
        {
            return NotFound();
        }
        return View(company);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            var userCompanies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId).Select(c => c.Id);
            if (!userCompanies.Contains(id))
            {
                return Forbid();
            }
        }
        var objFromDb = _unitOfWork.Companies.Get(c => c.Id == id, includeProperties: nameof(Category));
        if (objFromDb == null)
        {
            TempData["error"] = "Error while deleting company";
            return RedirectToAction(nameof(Index));
        }
        _unitOfWork.Companies.Remove(objFromDb);
        _unitOfWork.Save();
        _imageService.DeleteImage(objFromDb!.ImageUrl!);

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
