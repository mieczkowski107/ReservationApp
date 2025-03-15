using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using ReservationApp.Data;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Utility;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class ServiceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext dbContext;
    public ServiceController(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        this.dbContext = dbContext;
    }
    public IActionResult Index(int? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        List<Service> services;
        services = _unitOfWork.Services.GetAll(u => u.CompanyId == (int)id, includeProperties: nameof(Company)).ToList();
        if (!RoleService.IsAdmin(User))
        {
            services = services.Where(s => s.Company.OwnerId == userId).ToList();
        }

        var company = services.Any() ? services.First().Company : null;
        if (company == null)
        {
            return Forbid();
        }

        CompanyServiceVM companyServiceVM = new()
        {
            Company = company,
            Services = services
        };
        return View(companyServiceVM);
    }

    public IActionResult Create(int? CompanyId)
    {
        if (!CompanyId.HasValue)
        {
            return NotFound();
        }
        var userCompany = _unitOfWork.Companies.Get(u => u.Id == CompanyId);
        if (userCompany == null)
        {
            return NotFound();
        }
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (userCompany == null || userCompany.OwnerId != userId)
            {
                return Forbid();
            }
        }

        var newService = new Service()
        {
            CompanyId = (int)CompanyId,
            Company = userCompany
        };
        return View("Upsert", newService);

    }

    public IActionResult Edit(int? ServiceId)
    {
        if (!ServiceId.HasValue)
        {
            return NotFound();

        }
        var serviceObj = _unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: nameof(Company), tracked: false);
        if(serviceObj == null)
        {
            return NotFound();
        }

        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (serviceObj == null || serviceObj.Company.OwnerId != userId)
            {
                return Forbid();
            }
        }
     
        return View("Upsert", serviceObj);
    }

   

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Service? service)
    {
        if (ModelState.IsValid)
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            var services = _unitOfWork.Services.GetAll(u => u.CompanyId == service.CompanyId, includeProperties: nameof(Company), tracked: false).ToList();
            if (!RoleService.IsAdmin(User))
            {
                if(services.Any())
                {
                    if (services.First().Company.OwnerId != userId)
                    {
                        return Forbid();
                    }
                }
            }
            if (service.Id == 0)
            {
                _unitOfWork.Services.Add(service);
                TempData["success"] = "Service added succesffuly!";
            }
            else
            {
                _unitOfWork.Services.Update(service);
                TempData["success"] = "Service updated succesffuly!";
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index), new { Id = service.CompanyId });
        }
        else
        {
            return View(service);
        }
    }
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var serviceObj = _unitOfWork.Services.Get(u => u.Id == id, includeProperties: nameof(Company));
        if(serviceObj == null) 
        {
            return NotFound();
        }
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (serviceObj == null || serviceObj.Company.OwnerId != userId)
            {
                return Forbid();
            }
        }
        if (serviceObj == null)
        {
            return NotFound();
        }
        return View(serviceObj);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        if (id == null)
        {
            TempData["error"] = "Error while deleting company";
            return NotFound();
        }
        var serviceObj = _unitOfWork.Services.Get(u => u.Id == id, includeProperties: nameof(Company));
        if (serviceObj == null)
        {
            return NotFound();
        }
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            if (serviceObj == null || serviceObj.Company.OwnerId != userId)
            {
                return Forbid();
            }
        }
        if (serviceObj == null)
        {
            TempData["error"] = "Error while deleting company";
            return NotFound();
        }
        _unitOfWork.Services.Remove(serviceObj);
        _unitOfWork.Save();
        TempData["success"] = "Service deleted succesffuly!";
        return RedirectToAction(nameof(Index), new { Id = serviceObj.CompanyId });
    }


}

