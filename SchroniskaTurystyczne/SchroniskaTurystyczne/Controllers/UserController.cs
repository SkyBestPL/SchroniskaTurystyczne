using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;

namespace SchroniskaTurystyczne.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Shelter)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Shelter)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var isExhibitor = userRoles.Contains("Exhibitor");

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Reviews = user.Reviews.Select(r => new ReviewInfoViewModel
                {
                    Id = r.Id,
                    Date = r.Date,
                    Rating = r.Rating,
                    Contents = r.Contents,
                    Shelter = new ShelterBasicInfoViewModel
                    {
                        Id = r.Shelter.Id,
                        Name = r.Shelter.Name,
                        Location = $"{r.Shelter.City}, {r.Shelter.Country}"
                    }
                }).ToList(),
                VisitedShelters = user.Bookings
                    .Where(b => b.Ended && b.Valid)
                    .Select(b => new ShelterVisitedViewModel
                    {
                        Id = b.Shelter.Id,
                        Name = b.Shelter.Name,
                        Location = $"{b.Shelter.City}, {b.Shelter.Country}",
                        VisitDate = b.CheckOutDate
                    })
                    .Distinct()
                    .ToList()
            };

            if (isExhibitor && currentUser.IdShelter.HasValue)
            {
                viewModel.CurrentBookings = user.Bookings
                    .Where(b => !b.Ended && b.Valid && b.IdShelter == currentUser.IdShelter)
                    .Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        NumberOfPeople = b.NumberOfPeople,
                        Paid = b.Paid,
                        Verified = b.Verified
                    })
                    .ToList();
            }

            return View(viewModel);
        }
    }
}
