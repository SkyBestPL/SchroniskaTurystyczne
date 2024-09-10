using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class SavedRoute
    {
        [Key]
        public int Id { get; set; }
        public int IdGuest { get; set; }
        public string Name { get; set; }

        public virtual User Guest { get; set; }
        public virtual ICollection<RoutePoint> RoutePoints { get; set; }
    }
}
