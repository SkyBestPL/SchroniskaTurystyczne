using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchroniskaTurystyczne.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }
        public int? IdShelter { get; set; }

        [JsonIgnore]
        public Shelter? Shelter { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
        public virtual ICollection<Message>? SentMessages { get; set; }
        public virtual ICollection<Message>? ReceivedMessages { get; set; }
        public virtual ICollection<SavedRoute>? SavedRoutes { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
