using ReservationApp.Models;
namespace ReservationApp.Data.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Categories { get; }
        public ICompanyRepository Companies { get; }
        void Save();
    }
}
