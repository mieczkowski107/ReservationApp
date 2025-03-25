using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Data.Repository.Repositories;

namespace ReservationApp.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Categories { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public IServiceRepository Services { get; private set; }

        public IAppointmentRepository Appointments {  get; private set; }
        public IPaymentRepository Payment { get; set; }

        public INotificationRepository Notification { get; set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Categories = new CategoryRepository(_db);
            Companies = new CompanyRepository(_db);
            Services = new ServiceRepository(_db);
            Appointments = new AppointmentRepository(_db);
            Payment = new PaymentRepository(_db);
            Notification = new NotificationRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void BeginTransaction()
        {
            _db.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _db.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _db.Database.RollbackTransaction();
        }
    }
}
