using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _db;
        public ServiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
