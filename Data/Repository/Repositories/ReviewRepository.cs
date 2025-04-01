using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
