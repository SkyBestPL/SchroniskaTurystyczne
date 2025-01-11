using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string IdGuest { get; set; }
        [Required]
        public int NumberOfPeople { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public double Bill { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool Verified { get; set; } //true - właściciel zatwierdził tę rezerwację, false - właściciel jeszcze ani nie zatwierdził ani nie odrzucił danej rezerwacji
        public bool Valid { get; set; } //true - właściciel nie odrzucił tej rezerwacji, false - właściciel odrzucił tę rezerwację
        public bool Ended { get; set; } //true - okres rezerwacji się zakończył, false - rezerwacja jeszcze trwa

        public virtual AppUser Guest { get; set; }
        public virtual Shelter? Shelter { get; set; }
        public virtual ICollection<BookingRoom> BookingRooms { get; set; }
    }
}
