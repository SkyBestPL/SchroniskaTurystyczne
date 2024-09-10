using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Point
    {
        [Key]
        public int Id { get; set; }
        public int? IdShelter { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public double LocationLon { get; set; }
        public double LocationLat { get; set; }

        public virtual Shelter? Shelter { get; set; }
        public virtual ICollection<RoutePoint> RoutePoints { get; set; }
    }
}
