using ReservationApp.Models;
namespace ReservationApp.Data.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Categories { get; }
        public ICompanyRepository Companies { get; }
        public IServiceRepository Services { get; }
        public IAppointmentRepository Appointments { get; }
        void Save();
    }
}
