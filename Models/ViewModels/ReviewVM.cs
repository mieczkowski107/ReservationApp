namespace ReservationApp.Models.ViewModels;

public class ReviewOverallVM
{
    public string? CompanyName { get; set; }
    public int Rating { get; set; }
    public int Count { get; set; }
}

public class ReviewVM
{
    public string? ServiceName { get; set; }
    public string? Content { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserFirstName { get; set; }
}
