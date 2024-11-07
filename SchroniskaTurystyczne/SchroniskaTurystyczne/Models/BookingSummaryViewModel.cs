namespace SchroniskaTurystyczne.Models
{
    public class BookingSummaryViewModel
    {
        public string IdGuest { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<RoomSummary> Rooms { get; set; }
        public double TotalBill => Rooms.Sum(r => r.NumberOfPeople * r.PricePerNight * (CheckOutDate - CheckInDate).Days);
    }
}
