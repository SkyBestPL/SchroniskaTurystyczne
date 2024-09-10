using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SchroniskaTurystyczne.Models
{
    public class RoutePoint
    {
        [Key]
        public int IdRoute { get; set; }
        public int IdPoint { get; set; }

        public virtual SavedRoute SavedRoute { get; set; }
        public virtual Point Point { get; set; }
    }
}
