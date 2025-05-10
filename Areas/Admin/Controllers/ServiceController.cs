using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class ServiceController(IUnitOfWork _unitOfWork) : Controller
{
    public IActionResult Index(int id)
    {
        var userId = UserService.GetUserId(User);
        var isAdmin = UserService.IsAdmin(User);
        var services = _unitOfWork.Services.GetAll(u => u.CompanyId == (int)id && (u.Company!.OwnerId == userId || isAdmin), includeProperties: nameof(Company)).ToList();
        var company = _unitOfWork.Companies.Get(u => u.Id == id && (u.OwnerId == userId || UserService.IsAdmin(User)));

        if (company == null)
        {
            return Forbid();
        }

        CompanyServiceVM companyServiceVm = new()
        {
            Company = company,
            Services = services
        };
        return View(companyServiceVm);
    }

    public IActionResult Create(int? companyId)
    {
        if (!companyId.HasValue)
        {
            return NotFound();
        }
        var userCompany = _unitOfWork.Companies.Get(u => u.Id == companyId);
        if (userCompany == null)
        {
            return NotFound();
        }
        if (!UserService.IsAdmin(User))
        {
            var userId = UserService.GetUserId(User);
            if (userCompany.OwnerId != userId)
            {
                return Forbid();
            }
        }

        var newService = new Service()
        {
            CompanyId = (int)companyId,
            Company = userCompany
        };
        return View("Upsert", newService);

    }

    public IActionResult Edit(int? serviceId)
    {
        if (!serviceId.HasValue)
        {
            return NotFound();

        }
        var serviceObj = _unitOfWork.Services.Get(u => u.Id == serviceId, includeProperties: nameof(Company), tracked: false);
        if (serviceObj == null)
        {
            return NotFound();
        }

        if (!IsAuthorized(serviceObj, User))
        {
            return Forbid();
        }

        return View("Upsert", serviceObj);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Service? service)
    {
        if (ModelState.IsValid)
        {
            var userId = UserService.GetUserId(User);
            var services = _unitOfWork.Services.GetAll(u => u.CompanyId == service.CompanyId, includeProperties: nameof(Company), tracked: false).ToList();
            if (!UserService.IsAdmin(User))
            {
                if (services.Any())
                {
                    if (services.First().Company?.OwnerId != userId)
                    {
                        return Forbid();
                    }
                }
            }
            if (service!.Id == 0)
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
        if (serviceObj == null)
        {
            return NotFound();
        }
        if (!IsAuthorized(serviceObj, User))
        {
            return Forbid();
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
        if(!IsAuthorized(serviceObj, User))
        {
            return Forbid();
        }
      
        _unitOfWork.Services.Remove(serviceObj);
        _unitOfWork.Save();
        TempData["success"] = "Service deleted succesffuly!";
        return RedirectToAction(nameof(Index), new { Id = serviceObj.CompanyId });
    }

    private bool IsAuthorized(Service service, ClaimsPrincipal user)
    {
        var isAdmin = UserService.IsAdmin(user);
        return isAdmin || (service.Company!.OwnerId == UserService.GetUserId(user));
    }
}

