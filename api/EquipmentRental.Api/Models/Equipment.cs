namespace EquipmentRental.Api.Models
{
    // A machine in the fleet. The picture is a public photo linked by
    // URL, nothing is stored in the repository.
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<Rental> Rentals { get; set; } = new();
    }
}
