using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Models;

namespace ReservationApp.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<Report> Report { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Tattoo" },
            new Category { Id = 2, Name = "Barber" },
            new Category { Id = 3, Name = "Physiotherapy" }
        );
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "Ink Master",
                Address = "1234 Tattoo St",
                City = "Tattoo City",
                State = "Tattoo State",
                Zip = "12345",
                Phone = "123-456-7890",
                Email = "ink@gmail.com",
                CategoryId = 1,
                Description = "Tattoo Description",
                ImageUrl = @"\images\compnay\Temporary.jpg"
            },
            new Company
            {
                Id = 2,
                Name = "Barber Shop",
                Address = "1234 Barber St",
                City = "Barber City",
                State = "Barber State",
                Zip = "12345",
                Phone = "123-456-7890",
                Email = "barber@gmail.com",
                CategoryId = 2,
                Description = "Barber Shop Description",
                ImageUrl = @"\images\compnay\Temporary.jpg"
            },
            new Company
            {
                Id = 3,
                Name = "Physiotherapy Clinic",
                Address = "1234 Physiotherapy St",
                City = "Physiotherapy City",
                State = "Physiotherapy State",
                Zip = "12345",
                Phone = "123-456-7890",
                Email = "physio@gmail.com",
                CategoryId = 3,
                Description = "Physiotherapy Clinic Description",
                ImageUrl = @"\images\compnay\Temporary.jpg"
            });

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Tattoo Service 1",
                Description = "Tattoo Service Description",
                Price = 100,
                DurationMinutes = new TimeSpan(0, 30, 0),
                IsPrepaymentRequired = true,
                CompanyId = 1
            },
            new Service
            {
                Id = 2,
                Name = "Tattoo Service 2",
                Description = "Tattoo Service Description",
                Price = 200,
                DurationMinutes = new TimeSpan(0, 30, 0),
                IsPrepaymentRequired = true,
                CompanyId = 1
            },
            new Service
            {
                Id = 3,
                Name = "Tattoo Service 2",
                Description = "Tattoo Service Description",
                Price = 200,
                DurationMinutes = new TimeSpan(0, 30, 0),
                IsPrepaymentRequired = true,
                CompanyId = 1
            },
            new Service
            {
                Id = 4,
                Name = "Barber Service 1",
                Description = "Barber Service Description",
                Price = 50,
                DurationMinutes = new TimeSpan(0, 30, 0),
                IsPrepaymentRequired = true,
                CompanyId = 2
            },
            new Service
            {
                Id = 5,
                Name = "Barber Service 2",
                Description = "Barber Service Description",
                Price = 75,
                DurationMinutes = new TimeSpan(0, 45, 0),
                IsPrepaymentRequired = true,
                CompanyId = 2
            },
            new Service
            {
                Id = 6,
                Name = "Physiotherapy Service 1",
                Description = "Physiotherapy Service Description",
                Price = 150,
                DurationMinutes = new TimeSpan(1, 0, 0),
                IsPrepaymentRequired = true,
                CompanyId = 3,
            },
            new Service
            {
                Id = 7,
                Name = "Physiotherapy Service 2",
                Description = "Physiotherapy Service Description",
                Price = 200,
                DurationMinutes = new TimeSpan(0, 45, 0),
                IsPrepaymentRequired = true,
                CompanyId = 3
            }
            );
    }
}

