namespace ReservationApp.Utility;

public class RefundResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public RefundResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}