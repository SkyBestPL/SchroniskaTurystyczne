using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Shelter
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string IdExhibitor { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? ZipCode { get; set; }
        [Required]
        public string LocationLon { get; set; }
        [Required]
        public string LocationLat { get; set; }

        public virtual AppUser Exhibitor { get; set; }
        public virtual ICollection<Room>? Rooms { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
        public virtual ICollection<Point>? Points { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
