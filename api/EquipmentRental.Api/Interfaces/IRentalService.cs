using EquipmentRental.Api.Dtos;

namespace EquipmentRental.Api.Interfaces
{
    // What renting can end in, so the controller picks the status code.
    public enum RentalOutcome
    {
        Rented,
        NotFound,
        AlreadyRented,
    }

    public interface IRentalService
    {
        Task<(RentalOutcome Outcome, RentalDto? Rental)> RentAsync(int equipmentId, int days);
        Task<bool> CancelAsync(int equipmentId);
    }
}
