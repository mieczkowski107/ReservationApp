using ReservationApp.Models.ViewModels;

namespace ReservationApp.Services.Interfaces;

public interface IImageService
{
    public bool IsFileValid(IFormFile? File);
    public bool ImageUpload(CompanyVM companyVm, IFormFile? file);
    public void DeleteImage(string imageUrl);
}
