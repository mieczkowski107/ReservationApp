using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

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
        if (User.IsInRole(Role.Customer.ToString()))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (appointment.UserId != userId)
            {
                return Forbid();
            }
        }
        var domain = "https://localhost:7038/";

        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + "Customer/Appointment/Details?id=" + appointmentId,
            CancelUrl = domain + "Customer/Appointment/Confirmation/" + appointmentId,
            Mode = "payment",
            LineItems = new List<SessionLineItemOptions>()
        };
        var sessionLineItem = new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = (long)(appointment.Service.Price * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = appointment.Service.Name,
                },
            },
            Quantity = 1,
        };
        options.LineItems.Add(sessionLineItem);

        var service = new SessionService();
        var session = service.Create(options);
        _unitOfWork.Payment.UpdateStripePaymentID(appointmentId.Value, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();
        Response.Headers.Append("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult AppointmentRefund(int? appointmentId)
    {
        if (!appointmentId.HasValue)
        {
            return NotFound();
        }

        var appointment = _unitOfWork.Appointments.Get(
            u => u.Id == appointmentId,
            includeProperties: "Service.Company,User"
        );
        if (appointment == null)
        {
            return NotFound();
        }

        var userId = UserService.GetUserId(User);
        bool isAuthorized = UserService.IsAdmin(User) || appointment.Service!.Company!.OwnerId == userId;

        if (!isAuthorized)
        {
            return Forbid();
        }

        var result = _paymentService.RefundAppointment(appointmentId.Value);
        TempData[result.Success ? "success" : "error"] = result.Message;

        return RedirectToAction("Details", "Appointment", new { id = appointmentId });
    }
   

}


