using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
