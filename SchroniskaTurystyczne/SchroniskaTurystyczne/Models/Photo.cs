using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string? Name { get; set; }
        [Required]
        public byte[] PhotoData { get; set; }
        public byte[] ThumbnailData { get; set; }

        public virtual Shelter Shelter { get; set; }
    }
}
