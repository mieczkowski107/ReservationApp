using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;

namespace ReservationApp.Data.Repository.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    private readonly ApplicationDbContext _db;

    public ReportRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}
