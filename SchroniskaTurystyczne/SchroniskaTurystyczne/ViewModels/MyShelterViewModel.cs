using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.ViewModels
{
    public class MyShelterViewModel
    {
        public bool HasShelter { get; set; }
        public Shelter? Shelter { get; set; }
        public List<Room>? Rooms { get; set; }
        public Dictionary<string, int>? BookingStatistics { get; set; }
    }
}
