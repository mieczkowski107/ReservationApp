using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
