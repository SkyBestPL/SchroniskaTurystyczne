using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class BookingRoom
    {
        [Key]
        public int IdBooking { get; set; }
        public int IdRoom { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Room Room { get; set; }
    }
}
