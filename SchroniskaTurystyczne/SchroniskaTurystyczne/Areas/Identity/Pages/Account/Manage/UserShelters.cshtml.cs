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

            Shelters = await _context.Shelters
                .Where(s => s.IdExhibitor == user.Id)
                .ToListAsync();

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

        public async Task<IActionResult> OnPostDeleteAsync(int shelterId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var shelter = await _context.Shelters
                .Where(s => s.IdExhibitor == user.Id && s.Id == shelterId)
                .FirstOrDefaultAsync();

            if (shelter == null)
            {
                TempData["Error"] = "Nie znaleziono schroniska lub brak uprawnieñ do jego usuniêcia.";
                return RedirectToPage();
            }

            var rooms = _context.Rooms.Where(r => r.IdShelter == shelter.Id);
            foreach (var room in rooms)
            {
                var bookingRooms = _context.BookingRooms.Where(br => br.IdRoom == room.Id);
                _context.BookingRooms.RemoveRange(bookingRooms);
            }
            _context.Rooms.RemoveRange(rooms);

            var bookings = _context.Bookings.Where(b => b.BookingRooms.Any(br => br.Room.IdShelter == shelter.Id));
            _context.Bookings.RemoveRange(bookings);

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Schronisko zosta³o pomyœlnie usuniête.";
            return RedirectToPage();
        }
    }
}