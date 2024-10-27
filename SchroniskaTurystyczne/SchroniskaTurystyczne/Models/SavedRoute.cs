using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class SavedRoute
    {
        [Key]
        public int Id { get; set; }
        public string IdGuest { get; set; }
        public string Name { get; set; }

        public virtual AppUser Guest { get; set; }
        public virtual ICollection<Point> Points { get; set; }
    }
}
