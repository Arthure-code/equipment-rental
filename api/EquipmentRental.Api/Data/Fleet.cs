using EquipmentRental.Api.Models;

namespace EquipmentRental.Api.Data
{
    // The eight machines the database is created with. The photos are
    // public pictures on Unsplash, linked by URL under the Unsplash License;
    // the client picks the size it needs.
    public static class Fleet
    {
        private const string Photo = "https://images.unsplash.com/";

        public static readonly Equipment[] All =
        {
            new()
            {
                Id = 1,
                Name = "260 P-Tier",
                Category = "Earthmoving",
                Description = "Articulated dump truck. Net power 239 kW (321 hp) at 1,900 rpm. Rated payload 24,192 kg (53,334 lb).",
                DailyRate = 950,
                ImageUrl = Photo + "photo-1629807472592-2649bfa09f9c",
            },
            new()
            {
                Id = 2,
                Name = "460 P-Tier",
                Category = "Earthmoving",
                Description = "Articulated dump truck. Net power 359 kW (481 hp) at 1,700 rpm. Rated payload 41,820 kg (92,197 lb).",
                DailyRate = 1450,
                ImageUrl = Photo + "photo-1622645636770-11fbf0611463",
            },
            new()
            {
                Id = 3,
                Name = "744 P-Tier",
                Category = "Earthmoving",
                Description = "Wheel loader. Net power 236 kW (316 hp) at 1,500 rpm. Operating weight 24,907 to 26,630 kg (54,920 to 58,709 lb).",
                DailyRate = 1100,
                ImageUrl = Photo + "photo-1751054786365-4b02b690d301",
            },
            new()
            {
                Id = 4,
                Name = "844 P-Tier",
                Category = "Earthmoving",
                Description = "Wheel loader. Net power 311 kW (417 hp) at 1,400 rpm. Operating weight 33,943 to 34,777 kg (74,831 to 76,670 lb).",
                DailyRate = 1350,
                ImageUrl = Photo + "photo-1686945126682-35f9141dda15",
            },
            new()
            {
                Id = 5,
                Name = "1725",
                Category = "Planting",
                Description = "Planter with MaxEmerge 5 row units and 56 L (1.6 bu) or 106 L (3 bu) hoppers. Insecticide option on the 1.6 bu model.",
                DailyRate = 480,
                ImageUrl = Photo + "photo-1655048424318-2d56ccf9a51a",
            },
            new()
            {
                Id = 6,
                Name = "DR18",
                Category = "Planting",
                Description = "18-row planter with 56, 76, 91, 96, 100 or 102 cm row spacing. ExactEmerge, MaxEmerge 5e and MaxEmerge 5 row units.",
                DailyRate = 620,
                ImageUrl = Photo + "photo-1594691592772-4427be1ea26c",
            },
            new()
            {
                Id = 7,
                Name = "2230FH",
                Category = "Tillage",
                Description = "Field cultivator with working widths from 7.8 m (25 ft 6 in) to 21.2 m (69 ft 6 in). Follows the ground contour and stays level.",
                DailyRate = 390,
                ImageUrl = Photo + "photo-1696441567908-6a04d49e1350",
            },
            new()
            {
                Id = 8,
                Name = "2100",
                Category = "Tillage",
                Description = "Minimum-till ripper in a range of working widths, with main-frame extensions and gauge-wheel supports for a standard staggered configuration.",
                DailyRate = 350,
                ImageUrl = Photo + "photo-1718470684824-3c53a74523bc",
            },
        };
    }
}
