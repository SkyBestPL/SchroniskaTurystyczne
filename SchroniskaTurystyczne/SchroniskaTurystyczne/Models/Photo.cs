using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public virtual Shelter Shelter { get; set; }
    }
}
