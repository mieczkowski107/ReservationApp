using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Utility.Enums;
using System.Security.Claims;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class CompanyController(IUnitOfWork _unitOfWork, IWebHostEnvironment _hostEnvironment)
    : Controller
{
    public IActionResult Index()
    {
        List<Company> companies = new();
        if (User.IsInRole(Role.Admin.ToString()))
        {
            companies = _unitOfWork.Companies.GetAll(includeProperties: "Category").ToList();

        }
        else if (User.IsInRole(Role.CompanyManager.ToString()))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid ownerId);
            companies = _unitOfWork.Companies.GetAll(u => u.OwnerId == ownerId, includeProperties: "Category").ToList();
        }
        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
        if (!UserService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            var userCompanies = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId).Select(c => c.Id);
            if(id.HasValue)
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
    public IActionResult Upsert(CompanyVM companyVm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string imgPath = Path.Combine(wwwRootPath, @"images\company");
                if (!string.IsNullOrEmpty(companyVm.Company?.ImageUrl))
                {
                    string oldImagePath = Path.Combine(wwwRootPath, companyVm.Company.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(imgPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                companyVm.Company!.ImageUrl = @"\images\company\" + fileName;
            }
            else if (companyVm.Company!.ImageUrl == null)
            {
                companyVm.Company.ImageUrl = @"\images\company\Temporary.jpg";
            }

            if (companyVm.Company.OwnerId == null && User.IsInRole(Role.CompanyManager.ToString()))
            {
                Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid ownerId);
                companyVm.Company.OwnerId = ownerId;
            }

            if (companyVm.Company.Id == 0)
            {
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
        if(id == null)
        {
            return NotFound();
        }
        if (!UserService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
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
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
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
        TempData["success"] = "Company successfully deleted";
        return RedirectToAction(nameof(Index));
    }


    #region APICALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        if (User.IsInRole(Role.Admin.ToString()))
        {
            var allObj = _unitOfWork.Companies.GetAll();
            return Json(new { data = allObj });
        }
        else if (User.IsInRole(Role.CompanyManager.ToString()))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid ownerId);
            var allObj = _unitOfWork.Companies.GetAll(c => c.OwnerId == ownerId);
            return Json(new { data = allObj });
        }
        else
        {
            return Json(new { data = "" });
        }
    }

    #endregion
}
