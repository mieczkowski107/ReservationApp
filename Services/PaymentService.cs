using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility;
using ReservationApp.Utility.Enums;
using Stripe;

public class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
{
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
