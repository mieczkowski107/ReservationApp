using ReservationApp.Data.IRepository;
using ReservationApp.Models;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Data.Repository.IRepository;

public interface IPaymentRepository : IRepository<Payment>
{
    void UpdateStatus(int appointmentId, AppointmentStatus appointmentStatus, PaymentStatus? paymentStatus = null);
    void UpdateStripePaymentID(int appointmentId, string sessionId, string paymentIntentId);
}