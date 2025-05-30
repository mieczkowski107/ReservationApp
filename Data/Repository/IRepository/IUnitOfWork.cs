﻿using ReservationApp.Models;
namespace ReservationApp.Data.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Categories { get; }
        public ICompanyRepository Companies { get; }
        public IServiceRepository Services { get; }
        public IAppointmentRepository Appointments { get; }
        public IPaymentRepository Payment { get; }
        public INotificationRepository Notification { get; }
        public IReviewRepository Review { get; }
        public IReportRepository Report{ get; }

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void Save();
    }
}
