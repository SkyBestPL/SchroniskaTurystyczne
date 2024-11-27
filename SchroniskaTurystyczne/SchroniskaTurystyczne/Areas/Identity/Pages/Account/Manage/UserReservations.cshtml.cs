using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.Areas.Identity.Pages.Account.Manage
{
    public class UserReservationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserReservationsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Booking> CurrentBookings { get; set; } // Rezerwacje aktualne
        public List<Booking> PastBookings { get; set; } // Rezerwacje zakończone
        public List<Booking> RejectedBookings { get; set; } // Rezerwacje odrzucone

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Pobierz wszystkie rezerwacje użytkownika
            var bookings = await _context.Bookings
                .Where(r => r.IdGuest == user.Id)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(room => room.Shelter)
                .ToListAsync();

            // Podział rezerwacji na kategorie
            CurrentBookings = bookings
                .Where(b => !b.Ended && b.Valid)
                .ToList();

            PastBookings = bookings
                .Where(b => b.Ended && b.Valid)
                .ToList();

            RejectedBookings = bookings
                .Where(b => !b.Valid)
                .ToList();

            return Page();
        }
    }
}
