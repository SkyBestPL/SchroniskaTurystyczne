using Microsoft.AspNetCore.Identity;
using System.Data;
namespace SchroniskaTurystyczne.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual AppUser User { get; set; }
        public virtual Role Role { get; set; }
    }
}
