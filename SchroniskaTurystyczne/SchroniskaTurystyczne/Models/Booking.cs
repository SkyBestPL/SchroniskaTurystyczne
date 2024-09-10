using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int IdGuest { get; set; }
        public int NumberOfPeople { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; }

        public virtual User Guest { get; set; }
        public virtual ICollection<BookingRoom> BookingRooms { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
