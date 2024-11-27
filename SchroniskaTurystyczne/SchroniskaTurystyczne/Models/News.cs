namespace SchroniskaTurystyczne.Models
{
    public class News
    {
        public int Id { get; set; }
        public int IdShelter { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTime Date { get; set; }

        public virtual Shelter Shelter { get; set; }
    }
}
