using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string IdUser { get; set; }
        public string Date { get; set; }
        public int Rating { get; set; }
        public string Contents { get; set; }

        public virtual Shelter Shelter { get; set; }
        public virtual AppUser User { get; set; }
    }
}
