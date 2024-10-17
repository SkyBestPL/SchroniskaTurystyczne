using Microsoft.AspNetCore.Identity;

namespace SchroniskaTurystyczne.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<Shelter> Shelters { get; set; }
        public virtual ICollection<SavedRoute> SavedRoutes { get; set; }
    }
}
