using System.ComponentModel.DataAnnotations;

namespace EquipmentRental.Api.Dtos
{
    // What a client sends to rent: how many days, from now.
    public class RentalRequest
    {
        [Range(1, 30)]
        public int Days { get; set; }
    }

    // What it gets back: the rental, priced.
    public class RentalDto
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public int Days { get; set; }
        public decimal Total { get; set; }
    }
}
