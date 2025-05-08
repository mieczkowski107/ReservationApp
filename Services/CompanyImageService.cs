using Microsoft.Extensions.Hosting;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services.Interfaces;

namespace ReservationApp.Services;

public class CompanyImageService(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork) : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public bool IsFileValid(IFormFile file)
    {

        if (file.Length < 0 || file.Length > (2 * 1024 * 1024) || file == null) // 2MB
        {
            return false;
        }
        if (file.ContentType == "image/jpeg" || file.ContentType == "image/png" || file.ContentType == "image/jpg")
        {
            return true;
        }
        return false;
    }

    public bool ImageUpload(CompanyVM companyVm, IFormFile file)
    {
        if (!IsFileValid(file)) return false;
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string imgPath = Path.Combine(wwwRootPath, @"images\company");
            if (!string.IsNullOrEmpty(companyVm.Company!.ImageUrl))
            {
                DeleteImage(companyVm.Company!.ImageUrl);
            }
            using (var fileStream = new FileStream(Path.Combine(imgPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            companyVm.Company!.ImageUrl = @"\images\company\" + fileName;
        }
        else if (companyVm.Company!.ImageUrl == null)
        {
            companyVm.Company.ImageUrl = @"\images\company\Temporary.jpg";
        }
        return true;
    }

    public void DeleteImage(string imageUrl)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string deletePath = Path.Combine(wwwRootPath, imageUrl.TrimStart('\\'));
        if (File.Exists(deletePath))
        {
            File.Delete(deletePath);
        }
    }

}
