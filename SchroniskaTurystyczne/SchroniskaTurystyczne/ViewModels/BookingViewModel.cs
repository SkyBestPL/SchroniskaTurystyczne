namespace SchroniskaTurystyczne.ViewModels
{
    public class ShelterBookingViewModel
    {
        public int Id { get; set; }
        public string IdExhibitor { get; set; }
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

    public class PhotoBookingViewModel
    {
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string? Name { get; set; }
        public byte[] PhotoData { get; set; }
    }

    public class RoomBookingViewModel
    {
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public int IdType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double PricePerNight { get; set; }
        public int Capacity { get; set; }
        public bool HasConfirmedBooking { get; set; }
        public bool IsActive { get; set; }

        public RoomTypeBookingViewModel RoomType { get; set; }
        public List<BookingRoomBookingViewModel>? BookingRooms { get; set; }
        public List<FacilityBookingViewModel>? Facilities { get; set; }
        public List<RoomPhotoBookingViewModel>? RoomPhotos { get; set; }
    }

    public class RoomPhotoBookingViewModel
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public string? Name { get; set; }
        public byte[] PhotoData { get; set; }
    }

    public class RoomTypeBookingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class BookingRoomBookingViewModel
    {
        public int IdBooking { get; set; }
        public int IdRoom { get; set; }
        public int NumberOfPeople { get; set; }
    }

    public class FacilityBookingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
