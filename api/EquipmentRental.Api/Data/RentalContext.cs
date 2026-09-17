using EquipmentRental.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EquipmentRental.Api.Data
{
    public class RentalContext : DbContext
    {
        public RentalContext(DbContextOptions<RentalContext> options)
            : base(options) { }

        public DbSet<Equipment> Equipment => Set<Equipment>();
        public DbSet<Rental> Rentals => Set<Rental>();

        // Dates are stored in UTC and read back as UTC, so the JSON keeps
        // its "Z" and the browser converts them once.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var asUtc = new ValueConverter<DateTime, DateTime>(
                toDb => toDb,
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));

            modelBuilder.Entity<Equipment>().Property(e => e.DailyRate).HasPrecision(10, 2);
            modelBuilder.Entity<Equipment>().HasData(Fleet.All);
            modelBuilder.Entity<Rental>().Property(r => r.StartsOn).HasConversion(asUtc);
            modelBuilder.Entity<Rental>().Property(r => r.EndsOn).HasConversion(asUtc);
        }
    }
}
