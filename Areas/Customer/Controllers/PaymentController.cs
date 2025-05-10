using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using Stripe.Checkout;

namespace ReservationApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]

public class PaymentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;

    public PaymentController(IUnitOfWork unitOfWork ,IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AppointmentPayment(int? appointmentId)
    {
        if (!appointmentId.HasValue)
        {
            return NotFound();
        }
        var appointment = _unitOfWork.Appointments.Get(u => u.Id == appointmentId, includeProperties: "Service");
        if (appointment == null || appointment.Service == null)
        {
            return NotFound();
        }
        if (UserService.IsCustomer(User))
        {
            var userId = UserService.GetUserId(User).ToString();
            if (appointment.UserId != userId)
            {
                return Forbid();
            }
        }
        var session = _paymentService.CreateAppointmentSession(appointment);
        _unitOfWork.Payment.UpdateStripePaymentID(appointmentId.Value, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();
        Response.Headers.Append("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult AppointmentRefund(int appointmentId)
    {

        var appointment = _unitOfWork.Appointments.Get(
            u => u.Id == appointmentId,
            includeProperties: "Service.Company,User"
        );
        if (appointment == null)
        {
            return NotFound();
        }
        var payment = _unitOfWork.Payment.Get(u => u.AppointmentId == appointmentId);
        if(payment == null)
        {
            return NotFound();
        }

        var userId = UserService.GetUserId(User);
        bool isAuthorized = UserService.IsAdmin(User) || appointment.Service!.Company!.OwnerId == userId;

        if (!isAuthorized || payment.Status != PaymentStatus.Succeeded)
        {
            return Forbid();
        }
        
        var result = _paymentService.RefundAppointment(appointmentId);
        TempData[result.Success ? "success" : "error"] = result.Message;

        return RedirectToAction("Details", "Appointment", new {Area = "Admin" ,id = appointmentId });
    }
   

}


