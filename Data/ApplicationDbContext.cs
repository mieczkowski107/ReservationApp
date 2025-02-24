using Microsoft.EntityFrameworkCore;
using ReservationApp.Models;

namespace ReservationApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
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
                    Description = "Tattoo Description"
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
                    Description = "Barber Shop Description"
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
                    Description = "Physiotherapy Clinic Description"
                });
        }
    }
}

