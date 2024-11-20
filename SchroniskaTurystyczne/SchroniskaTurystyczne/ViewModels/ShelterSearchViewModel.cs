using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.ViewModels
{
    public class ShelterSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public List<int>? SelectedTagIds { get; set; }
        public List<int>? SelectedRoomTypeIds { get; set; }
        public List<TagViewModel> AvailableTags { get; set; }
        public List<RoomTypeViewModel> AvailableRoomTypes { get; set; }
        public IEnumerable<ShelterViewModel> Shelters { get; set; }
    }

    public class ShelterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Rating { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string LocationLon { get; set; }
        public string LocationLat { get; set; }
        public IEnumerable<TagViewModel> Tags { get; set; }
        public string? MainPhotoBase64 { get; set; }
    }

    public class TagViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RoomTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
