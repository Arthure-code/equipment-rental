namespace EquipmentRental.Api.Models
{
    // A rental starts the moment it is made and runs a whole number of
    // days. Cancelling keeps the row and clears the flag, so the history
    // stays.
    public class Rental
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool Active { get; set; }
    }
}
