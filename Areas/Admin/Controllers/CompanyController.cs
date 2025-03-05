using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var companies = _unitOfWork.Companies.GetAll(includeProperties: "Category").ToList();
        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
        var companyVm = new CompanyVM()
        {
            Company = new Company(),
            CategoryList = _unitOfWork.Categories.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            })
        };
        if (id == null)
        {
            return View(companyVm);
        }

        companyVm.Company = _unitOfWork.Companies.Get(c => c.Id == id);

        if (companyVm.Company == null)
        {
            return NotFound();
        }
        return View(companyVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company? company)
    {
        if (ModelState.IsValid)
        {
            if (company?.Id == 0 || company?.Id == null)
            {
                _unitOfWork.Companies.Add(company!);
                TempData["success"] = "Company added successfully!";
            }
            else
            {
                _unitOfWork.Companies.Update(company);
                TempData["success"] = "Company updated successfully!";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = "Something went wrong!";
            return RedirectToAction(nameof(Upsert), new { company?.Id });
            /*return View(company);*/
        }
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
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
        var objFromDb = _unitOfWork.Companies.Get(c => c.Id == id, includeProperties: nameof(Category));
        if (objFromDb == null)
        {
            TempData["error"] = "Error while deleting company";
            return RedirectToAction(nameof(Index));
            //return Json(new { success = false, message = "Error while deleting" });
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
        var allObj = _unitOfWork.Companies.GetAll();
        return Json(new { data = allObj });
    }
    
    #endregion
}
