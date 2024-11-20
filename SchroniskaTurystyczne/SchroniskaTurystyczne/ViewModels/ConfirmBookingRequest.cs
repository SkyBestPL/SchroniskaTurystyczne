namespace SchroniskaTurystyczne.ViewModels
{
    public class ConfirmBookingRequest
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<RoomBooking> Rooms { get; set; }

        public class RoomBooking
        {
            public int RoomId { get; set; }
            public int NumberOfPeople { get; set; }
        }
    }
}
