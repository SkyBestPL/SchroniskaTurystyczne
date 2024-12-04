using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class RoomPhoto
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public string? Name { get; set; }
        [Required]
        public byte[] PhotoData { get; set; }
        public virtual Room Room { get; set; }
    }
}
