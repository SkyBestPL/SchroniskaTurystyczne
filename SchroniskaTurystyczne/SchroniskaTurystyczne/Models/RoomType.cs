using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PricePerNight { get; set; }
        public int Capacity { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
