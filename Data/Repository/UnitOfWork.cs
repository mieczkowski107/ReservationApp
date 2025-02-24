using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Data.Repository.Repositories;

namespace ReservationApp.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Categories { get; private set; }
        public ICompanyRepository Companies { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Categories = new CategoryRepository(_db);
            Companies = new CompanyRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
