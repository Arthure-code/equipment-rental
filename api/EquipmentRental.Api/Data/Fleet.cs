using EquipmentRental.Api.Models;

namespace EquipmentRental.Api.Data
{
    // The eight machines the database is created with. The photos are
    // public pictures on Unsplash, linked by URL under the Unsplash License;
    // the client picks the size it needs.
    public static class Fleet
    {
        private const string Photos = "https://images.unsplash.com/";
        private const string Earthmoving = "Earthmoving";
        private const string Planting = "Planting";
        private const string Tillage = "Tillage";

        public static readonly Equipment[] All =
        {
            Machine(1, "260 P-Tier", Earthmoving, 950, "photo-1629807472592-2649bfa09f9c",
                "Articulated dump truck. Net power 239 kW (321 hp) at 1,900 rpm. Rated payload 24,192 kg (53,334 lb)."),
            Machine(2, "460 P-Tier", Earthmoving, 1450, "photo-1622645636770-11fbf0611463",
                "Articulated dump truck. Net power 359 kW (481 hp) at 1,700 rpm. Rated payload 41,820 kg (92,197 lb)."),
            Machine(3, "744 P-Tier", Earthmoving, 1100, "photo-1751054786365-4b02b690d301",
                "Wheel loader. Net power 236 kW (316 hp) at 1,500 rpm. Operating weight 24,907 to 26,630 kg (54,920 to 58,709 lb)."),
            Machine(4, "844 P-Tier", Earthmoving, 1350, "photo-1686945126682-35f9141dda15",
                "Wheel loader. Net power 311 kW (417 hp) at 1,400 rpm. Operating weight 33,943 to 34,777 kg (74,831 to 76,670 lb)."),
            Machine(5, "1725", Planting, 480, "photo-1655048424318-2d56ccf9a51a",
                "Planter with MaxEmerge 5 row units and 56 L (1.6 bu) or 106 L (3 bu) hoppers. Insecticide option on the 1.6 bu model."),
            Machine(6, "DR18", Planting, 620, "photo-1594691592772-4427be1ea26c",
                "18-row planter with 56, 76, 91, 96, 100 or 102 cm row spacing. ExactEmerge, MaxEmerge 5e and MaxEmerge 5 row units."),
            Machine(7, "2230FH", Tillage, 390, "photo-1696441567908-6a04d49e1350",
                "Field cultivator with working widths from 7.8 m (25 ft 6 in) to 21.2 m (69 ft 6 in). Follows the ground contour and stays level."),
            Machine(8, "2100", Tillage, 350, "photo-1718470684824-3c53a74523bc",
                "Minimum-till ripper in a range of working widths, with main-frame extensions and gauge-wheel supports for a standard staggered configuration."),
        };

        private static Equipment Machine(int id, string name, string category, decimal dailyRate, string photo, string description)
        {
            return new Equipment
            {
                Id = id,
                Name = name,
                Category = category,
                Description = description,
                DailyRate = dailyRate,
                ImageUrl = Photos + photo,
            };
        }
    }
}
