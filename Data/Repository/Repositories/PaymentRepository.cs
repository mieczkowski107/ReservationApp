using Microsoft.EntityFrameworkCore;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Data.Repository.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateStatus(int appointmentId, AppointmentStatus appointmentStatus, PaymentStatus? paymentStatus = null)
        {
            var payment = _db.Payment.Include(Payment => Payment.Appointment).FirstOrDefault(u => u.AppointmentId == appointmentId);
            if (payment == null)
            {
                return;
            }
            if(payment.Appointment == null)
            {
                return;
            }
            if (payment != null)
            {
                payment.Appointment.Status = appointmentStatus;
                if (paymentStatus != null)
                {
                    payment.Status = paymentStatus.Value;
                }
            }

        }

        public void UpdateStripePaymentID(int appointmentId, string sessionId, string paymentIntentId)
        {
            var payment = _db.Payment.FirstOrDefault(u => u.AppointmentId == appointmentId);
            if (payment == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(sessionId))
            {
                payment.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                payment.PaymentIntentId = paymentIntentId;
                payment.CompletedDate = DateTime.UtcNow;
            }
        }
    }
}
