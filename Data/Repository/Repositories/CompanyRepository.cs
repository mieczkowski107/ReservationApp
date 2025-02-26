using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public Dictionary<string, List<Company>> GetCompaniesByCategory()
        {
            return _db.Companies
                .GroupBy(c => c.Category.Name)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
