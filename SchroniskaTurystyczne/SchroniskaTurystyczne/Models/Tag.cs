using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ShelterTag> ShelterTags { get; set; }
    }
}
