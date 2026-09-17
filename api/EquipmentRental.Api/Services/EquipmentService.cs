using EquipmentRental.Api.Data;
using EquipmentRental.Api.Dtos;
using EquipmentRental.Api.Interfaces;
using EquipmentRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Api.Services
{
    // Reads the fleet and says, for each machine, whether a rental covers
    // this very moment. The whole projection runs in the database.
    public class EquipmentService : IEquipmentService
    {
        private readonly RentalContext _context;

        public EquipmentService(RentalContext context)
        {
            _context = context;
        }

        public Task<List<EquipmentDto>> GetAllAsync()
        {
            return Project(_context.Equipment.OrderBy(e => e.Id)).ToListAsync();
        }

        public Task<EquipmentDto?> GetAsync(int id)
        {
            return Project(_context.Equipment.Where(e => e.Id == id)).FirstOrDefaultAsync();
        }

        private static IQueryable<EquipmentDto> Project(IQueryable<Equipment> equipment)
        {
            var now = DateTime.UtcNow;
            return equipment.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Description = e.Description,
                DailyRate = e.DailyRate,
                ImageUrl = e.ImageUrl,
                Available = !e.Rentals.Any(r => r.Active && r.StartsOn <= now && r.EndsOn >= now),
                RentedFrom = e.Rentals
                    .Where(r => r.Active && r.StartsOn <= now && r.EndsOn >= now)
                    .Select(r => (DateTime?)r.StartsOn)
                    .FirstOrDefault(),
                RentedUntil = e.Rentals
                    .Where(r => r.Active && r.StartsOn <= now && r.EndsOn >= now)
                    .Select(r => (DateTime?)r.EndsOn)
                    .FirstOrDefault(),
            });
        }
    }
}
