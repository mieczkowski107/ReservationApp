using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController(IUnitOfWork unitOfWork) : Controller
{
    public IActionResult Index()
    {
        var categories = unitOfWork.Categories.GetAll().ToList();
        return View(categories);
    }

    public IActionResult Upsert(int? categoryId)
    {
        if (categoryId == null)
        {
            return View(new Category());
        }

        var category = unitOfWork.Categories.Get(c => c.Id == categoryId);

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
                unitOfWork.Categories.Add(category!);
                TempData["success"] = "Category added succesffuly!";
            }
            else
            {
                unitOfWork.Categories.Update(category);
                TempData["success"] = "Category updated succesffuly!";
            }
            unitOfWork.Save();
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
        var category = unitOfWork.Categories.Get(c => c.Id == categoryId);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var objFromDb = unitOfWork.Categories.Get(c => c.Id == id);
        var companies = unitOfWork.Companies.GetAll(u=>u.CategoryId == id, includeProperties:nameof(Category));
        if (objFromDb == null)
        {
            TempData["error"] = "Error while deleting";
            return RedirectToAction(nameof(Index));
            //return Json(new { success = false, message = "Error while deleting" });
        }
        if(!companies.Any())
        {
            unitOfWork.Categories.Remove(objFromDb);
            unitOfWork.Save();
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
        var allObj = unitOfWork.Categories.GetAll();
        return Json(new { data = allObj });
    }
    
    #endregion
}
