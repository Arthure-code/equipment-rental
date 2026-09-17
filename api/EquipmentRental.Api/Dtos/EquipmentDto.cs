namespace EquipmentRental.Api.Dtos
{
    // A machine as the catalogue shows it: its card, and whether it can
    // be rented right now. The two dates exist only while it is rented.
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool Available { get; set; }
        public DateTime? RentedFrom { get; set; }
        public DateTime? RentedUntil { get; set; }
    }
}
