using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
