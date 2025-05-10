using ReservationApp.Models;
using ReservationApp.Utility;
using Stripe.Checkout;

namespace ReservationApp.Services.Interfaces;

public interface IPaymentService
{
    Session CreateAppointmentSession(Appointment appointment);
    RefundResult RefundAppointment(int appointmentId);
}
