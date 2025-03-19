namespace ReservationApp.Utility.Enums;

public enum PaymentStatus
{
    Pending,   // The payment is being processed
    Succeeded, // The payment was successful
    Failed,    // The payment was declined or failed
    Refunded,  // The payment was refunded
    Cancelled  // The payment was canceled before being completed.
}
