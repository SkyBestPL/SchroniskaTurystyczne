using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SchroniskaTurystyczne.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        public Role Role { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
    }
}
