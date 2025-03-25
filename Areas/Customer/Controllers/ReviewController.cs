using Microsoft.AspNetCore.Mvc;
using ReservationApp.Data.Repository.IRepository;
using System.Runtime.CompilerServices;

namespace ReservationApp.Areas.Customer.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int companyId)
        {

            return View();
        }
    }
}
