using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchroniskaTurystyczne.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public int IdType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double PricePerNight { get; set; }
        public int Capacity { get; set; }

        public virtual Shelter Shelter { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<BookingRoom>? BookingRooms { get; set; }
        public virtual ICollection<Facility>? Facilities { get; set; }
        public virtual ICollection<RoomPhoto>? RoomPhotos { get; set; }
        public virtual ICollection<Point>? Points { get; set; }

        [NotMapped]
        public string SelectedFacilities { get; set; }
    }
}
