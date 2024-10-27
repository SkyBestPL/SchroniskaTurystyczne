using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string IdGuest { get; set; }
        [Required]
        public int NumberOfPeople { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public double Bill { get; set; }
        public bool Paid { get; set; }
        public int? PaymentDate { get; set; }
        public bool Verified { get; set; }

        public virtual AppUser Guest { get; set; }
        public virtual ICollection<BookingRoom> BookingRooms { get; set; }
    }
}
