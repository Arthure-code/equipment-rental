using EquipmentRental.Api.Data;
using EquipmentRental.Api.Dtos;
using EquipmentRental.Api.Interfaces;
using EquipmentRental.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Api.Services
{
    public class RentalService : IRentalService
    {
        private readonly RentalContext _context;

        public RentalService(RentalContext context)
        {
            _context = context;
        }

        // A machine can only be out once at a time: a rental that overlaps
        // the requested days is refused rather than stacked.
        public async Task<(RentalOutcome Outcome, RentalDto? Rental)> RentAsync(int equipmentId, int days)
        {
            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment is null) return (RentalOutcome.NotFound, null);

            var startsOn = DateTime.UtcNow;
            var endsOn = startsOn.AddDays(days);
            var overlaps = await _context.Rentals.AnyAsync(r =>
                r.EquipmentId == equipmentId && r.Active && r.StartsOn < endsOn && r.EndsOn > startsOn);
            if (overlaps) return (RentalOutcome.AlreadyRented, null);

            var rental = new Rental
            {
                EquipmentId = equipmentId,
                StartsOn = startsOn,
                EndsOn = endsOn,
                Active = true,
            };
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return (RentalOutcome.Rented, new RentalDto
            {
                Id = rental.Id,
                EquipmentId = equipmentId,
                StartsOn = startsOn,
                EndsOn = endsOn,
                Days = days,
                Total = equipment.DailyRate * days,
            });
        }

        // Cancels the rental in progress, if there is one.
        public async Task<bool> CancelAsync(int equipmentId)
        {
            var now = DateTime.UtcNow;
            var current = await _context.Rentals
                .Where(r => r.EquipmentId == equipmentId && r.Active && r.EndsOn >= now)
                .OrderByDescending(r => r.StartsOn)
                .FirstOrDefaultAsync();
            if (current is null) return false;

            current.Active = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
