using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Shelter
    {
        [Key]
        public int Id { get; set; }
        public string IdExhibitor { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public string LocationLon { get; set; }
        public string LocationLat { get; set; }

        public AppUser Exhibitor { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ShelterTag> ShelterTags { get; set; }
        public ICollection<Point> Points { get; set; }
    }
}
