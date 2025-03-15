using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Utility;
using System.Configuration;


namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var Categories = _unitOfWork.Categories.GetAll().ToList();
        return View(Categories);
    }

    public IActionResult Upsert(int? categoryId)
    {
        if (categoryId == null)
        {
            return View(new Category());
        }

        var category = _unitOfWork.Categories.Get(c => c.Id == categoryId);

        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Category? category)
    {
        if (ModelState.IsValid)
        {
            if (category?.Id == 0 || category?.Id == null)
            {
                _unitOfWork.Categories.Add(category!);
                TempData["success"] = "Category added succesffuly!";
            }
            else
            {
                _unitOfWork.Categories.Update(category);
                TempData["success"] = "Category updated succesffuly!";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        else
        {
            return View(category);
        }
    }

    public IActionResult Delete(int? categoryId)
    {
        if (categoryId == null)
        {
            return NotFound();
        }
        var category = _unitOfWork.Categories.Get(c => c.Id == categoryId);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var objFromDb = _unitOfWork.Categories.Get(c => c.Id == id);
        var companies = _unitOfWork.Companies.GetAll(u=>u.CategoryId == id, includeProperties:nameof(Category));
        if (objFromDb == null)
        {
            TempData["error"] = "Error while deleting";
            return RedirectToAction(nameof(Index));
            //return Json(new { success = false, message = "Error while deleting" });
        }
        if(!companies.Any())
        {
            _unitOfWork.Categories.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Category successfully deleted";
        }
        else
        {
            TempData["error"] = "Can not delete a category if it is assigned to any company";
        }

        
        return RedirectToAction(nameof(Index));
    }


    #region APICALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var allObj = _unitOfWork.Categories.GetAll();
        return Json(new { data = allObj });
    }
    
    #endregion
}
