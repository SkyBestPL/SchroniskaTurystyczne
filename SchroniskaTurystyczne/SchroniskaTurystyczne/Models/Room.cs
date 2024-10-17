using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public int IdType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public virtual Shelter Shelter { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<BookingRoom> BookingRooms { get; set; }
    }
}
