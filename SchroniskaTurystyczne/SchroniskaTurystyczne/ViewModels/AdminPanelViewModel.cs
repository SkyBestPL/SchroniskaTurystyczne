namespace SchroniskaTurystyczne.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<UserAdminViewModel> Users { get; set; }
        public List<ShelterAdminViewModel> Shelters { get; set; }
        public List<TagAdminViewModel> Tags { get; set; }
        public List<FacilityAdminViewModel> Facilities { get; set; }
    }

    public class ShelterAdminViewModel
    {
        public int Id { get; set; }
        public string IdExhibitor { get; set; }
        public string OwnerName { get; set; }
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public int AmountOfReviews { get; set; }
        public bool ConfirmedShelter { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? ZipCode { get; set; }
        public string LocationLon { get; set; }
        public string LocationLat { get; set; }

        public List<RoomBookingViewModel>? Rooms { get; set; }
        public List<PhotoBookingViewModel>? Photos { get; set; }
    }

    public class UserAdminViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class FacilityAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TagAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
