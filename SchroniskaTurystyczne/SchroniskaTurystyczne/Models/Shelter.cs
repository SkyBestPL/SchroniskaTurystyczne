using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Shelter
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string IdExhibitor { get; set; }
        [Required(ErrorMessage = "Wybór kategorii jest obowiązkowy.")]
        public int IdCategory { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Nazwa schroniska nie może przekraczać 100 znaków.")]
        public string Name { get; set; }
        [StringLength(5000, ErrorMessage = "Opis schroniska nie może przekraczać 5000 znaków.")]
        public string? Description { get; set; }
        public double? Rating { get; set; } //średnia ocen
        public int AmountOfReviews { get; set; } //ilość ocen
        public bool ConfirmedShelter { get; set; }
        [StringLength(100, ErrorMessage = "Nazwa kraju nie może przekraczać 100 znaków.")]
        public string? Country { get; set; }
        [StringLength(100, ErrorMessage = "Nazwa miasta nie może przekraczać 100 znaków.")]
        public string? City { get; set; }
        [StringLength(200, ErrorMessage = "Nazwa ulicy nie może przekraczać 200 znaków.")]
        public string? Street { get; set; }
        [StringLength(50, ErrorMessage = "Numer ulicy nie może przekraczać 50 znaków.")]
        public string? StreetNumber { get; set; }
        [StringLength(20, ErrorMessage = "Kod pocztowy nie może przekraczać 20 znaków.")]
        public string? ZipCode { get; set; }
        [Required]
        public string LocationLon { get; set; }
        [Required]
        public string LocationLat { get; set; }

        public virtual AppUser Exhibitor { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Room>? Rooms { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
        public virtual ICollection<Point>? Points { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
