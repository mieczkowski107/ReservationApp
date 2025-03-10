using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,CompanyManager")]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
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
                if(!string.IsNullOrEmpty(companyVm?.Company.ImageUrl))
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
                companyVm!.Company.ImageUrl = @"\images\company\" + fileName;
            }
            else if(companyVm.Company.ImageUrl == null)
            {
                companyVm.Company.ImageUrl = @"\images\company\Temporary.jpg";
            }
            if (companyVm?.Company.Id == 0 || companyVm?.Company.Id == null)
            {
                _unitOfWork.Companies.Add(companyVm!.Company!);
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
            return RedirectToAction(nameof(Upsert), new { companyVm?.Company.Id });
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
