namespace SchroniskaTurystyczne.Models
{
    public class RoomPhoto
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public string? Name { get; set; }
        public string Path { get; set; }
        public virtual Room Room { get; set; }
    }
}
