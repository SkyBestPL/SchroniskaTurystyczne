using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchroniskaTurystyczne.Areas.Identity.Pages.Account.Manage
{
    public class UserSheltersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserSheltersModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Shelter> Shelters { get; set; }
        public Dictionary<int, int> ApprovedBookingsCount { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> PendingBookingsCount { get; set; } = new Dictionary<int, int>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Pobranie schronisk wystawionych przez u¿ytkownika
            Shelters = await _context.Shelters
                .Where(s => s.IdExhibitor == user.Id)
                .ToListAsync();

            // Liczba zatwierdzonych i niezatwierdzonych rezerwacji dla ka¿dego schroniska
            foreach (var shelter in Shelters)
            {
                var approvedBookings = await _context.Bookings
                    .Where(b => b.Verified && b.BookingRooms.Any(br => br.Room.Shelter.Id == shelter.Id))
                    .ToListAsync();

                var pendingBookings = await _context.Bookings
                    .Where(b => !b.Verified && b.BookingRooms.Any(br => br.Room.Shelter.Id == shelter.Id))
                    .ToListAsync();

                ApprovedBookingsCount[shelter.Id] = approvedBookings.Count;
                PendingBookingsCount[shelter.Id] = pendingBookings.Count;
            }

            return Page();
        }
    }
}