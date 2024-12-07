using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchroniskaTurystyczne.Models
{
    public class RoomPhoto
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public string? Name { get; set; }
        [Required]
        public byte[] PhotoData { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }
    }
}
