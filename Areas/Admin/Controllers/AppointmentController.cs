using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models.ViewModels;
using ReservationApp.Utility.Enums;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "CompanyManager,Admin")]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public AppointmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index(int Id)
    {
        return View();
    }

    public IActionResult Details(int? Id)
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        var appointment = _unitOfWork.Appointments.Get(u => u.Id == Id && u.Service.Company.OwnerId == userId, includeProperties: "Service.Company,User");
        return View();
    }

    #region APICALLS
    public IActionResult GetCompanyAppointments(int Id)
    {
        List<CompanyAppointments> companyAppointments = [];
        var appointments = _unitOfWork.Appointments.GetAll(
            u => u.Service.CompanyId == Id,
            includeProperties: "Service.Company,User").ToList();
        if (User.IsInRole(Role.CompanyManager.ToString()))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
            appointments = appointments.Where(u => u.Service.Company.OwnerId == userId).ToList();
        }

        foreach (var appointment in appointments)
        {
            var companyAppointment = new CompanyAppointments()
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Time = appointment.Time,
                AppointmentStatus = appointment.Status,
                ServiceName = appointment.Service.Name,
                UserFirstName = appointment.User.FirstName,
                UserLastName = appointment.User.LastName,
                UserEmail = appointment.User.Email,
                UserPhoneNumber = appointment.User.PhoneNumber,
                IsPrepaymentRequired = appointment.Service.IsPrepaymentRequired

            };
            companyAppointments.Add(companyAppointment);
        }
        return Json(new { data = companyAppointments });
    }
    #endregion
}
