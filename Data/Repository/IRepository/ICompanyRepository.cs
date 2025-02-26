using ReservationApp.Data.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Dictionary<string, List<Company>> GetCompaniesByCategory();
    }
}