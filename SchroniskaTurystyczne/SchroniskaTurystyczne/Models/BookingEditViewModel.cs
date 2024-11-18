namespace SchroniskaTurystyczne.Models
{
    public class BookingEditViewModel
    {
        public int BookingId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Shelter Shelter { get; set; }
        public Dictionary<int, int> SelectedRooms { get; set; }
    }

    public class UpdateBookingRequest
    {
        public int BookingId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<RoomBookingRequest> Rooms { get; set; }
    }

    public class RoomBookingRequest
    {
        public int RoomId { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
