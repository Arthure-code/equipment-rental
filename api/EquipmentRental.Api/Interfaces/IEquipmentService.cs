using EquipmentRental.Api.Dtos;

namespace EquipmentRental.Api.Interfaces
{
    public interface IEquipmentService
    {
        Task<List<EquipmentDto>> GetAllAsync();
        Task<EquipmentDto?> GetAsync(int id);
    }
}
