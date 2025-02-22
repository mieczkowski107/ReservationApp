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
           
        }
    }
}

