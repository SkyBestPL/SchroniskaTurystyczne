using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class BookingRoom
    {
        public int IdBooking { get; set; }
        public int IdRoom { get; set; }
        [Required]
        public int NumberOfPeople { get; set; } //osoby rezerwujące dany pokój w danej rezerwacj

        public virtual Booking Booking { get; set; }
        public virtual Room Room { get; set; }
    }
}
