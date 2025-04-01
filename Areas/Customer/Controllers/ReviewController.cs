using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Utility.Enums;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
public class ReviewController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(int? companyId)
    {
        if (!companyId.HasValue)
        {
            return NotFound();
        }
        var reviews = _unitOfWork.Review.GetAll(filter: r => r.Appointment.Service.CompanyId == companyId, includeProperties: "Appointment.Service.Company").ToList();

        return View(reviews);
    }

    [Authorize]
    public IActionResult Create(int appointmentId)
    {
        var review = _unitOfWork.Review.Get(filter: r => r.AppointmentId == appointmentId, includeProperties: nameof(Appointment));
        if (review != null || review.Appointment.Status != AppointmentStatus.Confirmed)
        {
            TempData["Error"] = "You cannot reviewed this appointment";
            return RedirectToAction("UserAppointments", "Appointment");
        }
        var newReview = new Review
        {
            AppointmentId = appointmentId
        };
        return View(newReview);
    }

    [HttpPost]
    [Authorize]
    [ActionName("Create")]
    public IActionResult CreatePost(Review review)
    {
        if (!ModelState.IsValid)
        {
            return View(review);
        }
        _unitOfWork.Review.Add(review);
        _unitOfWork.Save();
        TempData["Success"] = "You review has been added successfully!";
        return RedirectToAction("UserAppointments","Appointment");
    }

    [Authorize]
    public IActionResult Delete(int id)
    {
        var review = _unitOfWork.Review.Get(filter: r => r.Id == id);
        if (review == null)
        {
            return NotFound();
        }
        _unitOfWork.Review.Remove(review);
        _unitOfWork.Save();
        return RedirectToAction("Index",nameof(AppointmentController) ,new { companyId = review.Appointment.Service.CompanyId });
    }

    [HttpGet]
    public IActionResult GetCompanyReviews(int companyId)
    {
        var reviews = _unitOfWork.Review.GetAll(filter: r => r.Appointment.Service.CompanyId == companyId,
                                                includeProperties: "Appointment.User,Appointment.Service")
                                                .OrderByDescending(r => r.CreatedAt)
                                                .Take(50)
                                                .ToList();
        var reviewsDTO = reviews.Select(r => new ReviewVM
            {
                ServiceName = r.Appointment.Service.Name,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt.Date,
                UserFirstName = r.Appointment.User.FirstName,
        }).ToList();
        return Json(new { data = reviewsDTO });
    }


}
