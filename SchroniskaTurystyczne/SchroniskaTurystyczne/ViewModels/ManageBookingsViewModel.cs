namespace SchroniskaTurystyczne.ViewModels
{
    public class ManageBookingsViewModel
    {
        public int ShelterId { get; set; }
        public string ShelterName { get; set; }
        public List<BookingDetailsViewModel> PendingBookings { get; set; }
        public List<BookingDetailsViewModel> ConfirmedBookings { get; set; }
        public List<BookingDetailsViewModel> CompletedBookings { get; set; }
    }

    public class BookingDetailsViewModel
    {
        public int BookingId { get; set; }
        public string IdUser { get; set; }
        public string GuestName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public double TotalPrice { get; set; }
        public bool Paid { get; set; }
        public List<RoomBookingDetail> Rooms { get; set; }
    }

    public class RoomBookingDetail
    {
        public string RoomName { get; set; }
        public int NumberOfPeople { get; set; }
        public double PricePerNight { get; set; }
    }
}
