using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class ServiceController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ServiceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var services = _unitOfWork.Services.GetAll(u => u.CompanyId == (int)id, includeProperties: nameof(Company)).ToList();
        var company = _unitOfWork.Companies.Get(u => u.Id == id);
        if(company == null)
        {
            return NotFound();
        }

        CompanyServiceVM companyServiceVM = new()
        {
            Company = company,
            Services = services
        };
        return View(companyServiceVM);
    }

    public IActionResult Upsert(int? ServiceId, int? CompanyId)
    {
        if (ServiceId == null || ServiceId == 0)
        {
            var newService = new Service()
            {
                CompanyId = (int)CompanyId,
                Company = _unitOfWork.Companies.Get(u => u.Id == (int)CompanyId)
            };
            return View(newService);
        }
        var serviceObj = _unitOfWork.Services.Get(u => u.Id == ServiceId, includeProperties: nameof(Company));
        if (serviceObj == null)
        {
            return NotFound();
        }
        return View(serviceObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Service? service)
    {
        if (ModelState.IsValid)
        {
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
        var service = _unitOfWork.Services.Get(c => c.Id == id, includeProperties: nameof(Company));
        if (service == null)
        {
            return NotFound();
        }
        return View(service);
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
        var service = _unitOfWork.Services.Get(u => u.Id == id, includeProperties: nameof(Company));
        if (service == null)
        {
            TempData["error"] = "Error while deleting company";
            return NotFound();
        }
        _unitOfWork.Services.Remove(service);
        _unitOfWork.Save();
        TempData["success"] = "Service deleted succesffuly!";
        return RedirectToAction(nameof(Index), new { Id = service.CompanyId });
    }


}

