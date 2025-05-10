using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility;
using ReservationApp.Utility.Enums;
using Stripe;
using Stripe.Checkout;

public class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
{
    public Session CreateAppointmentSession(Appointment appointment)
    {
        var domain = "https://localhost:7038/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + "Customer/Appointment/Details?id=" + appointment.Id,
            CancelUrl = domain + "Customer/Appointment/Confirmation/" + appointment.Id,
            Mode = "payment",
            LineItems = new List<SessionLineItemOptions>()
        };
        var sessionLineItem = new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = (long)(appointment.Service!.Price * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = appointment.Service.Name,
                },
            },
            Quantity = 1,
        };
        options.LineItems.Add(sessionLineItem);

        var service = new SessionService();
        return service.Create(options);
    }

    public RefundResult RefundAppointment(int appointmentId)
    {
        var payment = unitOfWork.Payment.Get(u => u.AppointmentId == appointmentId);
        if (payment == null)
        {
            return new RefundResult(false, "Payment not found.");
        }

        if (payment.Status != PaymentStatus.Succeeded)
        {
            return new RefundResult(false, "No successful payment found to refund.");
        }

        var options = new RefundCreateOptions
        {
            Reason = RefundReasons.RequestedByCustomer,
            PaymentIntent = payment.PaymentIntentId,
        };

        var refundService = new RefundService();
        refundService.Create(options);

        unitOfWork.Payment.UpdateStatus(appointmentId, AppointmentStatus.Cancelled, PaymentStatus.Refunded);
        unitOfWork.Save();

        return new RefundResult(true, "Refund processed successfully.");
    }
}
