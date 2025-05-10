using ReservationApp.Utility;

namespace ReservationApp.Services.Interfaces;

public interface IPaymentService
{
    RefundResult RefundAppointment(int appointmentId);
}
