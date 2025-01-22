using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;

namespace SchroniskaTurystyczne.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var roleTranslations = new Dictionary<string, string>
            {
                { "Exhibitor", "Właściciel" },
                { "Guest", "Gość" },
                { "Admin", "Admin" }
            };

            var users = await _userManager.Users
                .Where(user => user.Id != currentUserId)
                .Select(user => new UserAdminViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roleTranslations.GetValueOrDefault(
                        user.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault(),
                        "-"
                    )
                })
                .ToListAsync();

            var shelters = await _context.Shelters
                .Include(s => s.Exhibitor)
                .Select(shelter => new ShelterAdminViewModel
                {
                    Id = shelter.Id,
                    Name = shelter.Name,
                    OwnerName = $"{shelter.Exhibitor.FirstName} {shelter.Exhibitor.LastName}",
                    ConfirmedShelter = shelter.ConfirmedShelter
                })
                .ToListAsync();

            var tags = await _context.Tags
                .Select(tag => new TagAdminViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name
                })
                .ToListAsync();

            var facilities = await _context.Facilities
                .Select(fac => new FacilityAdminViewModel
                {
                    Id = fac.Id,
                    Name = fac.Name
                })
                .ToListAsync();

            var viewModel = new AdminPanelViewModel
            {
                Users = users,
                Shelters = shelters,
                Tags = tags,
                Facilities = facilities
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var reviews = _context.Reviews.Where(r => r.IdUser == user.Id);
            _context.Reviews.RemoveRange(reviews);

            var bookings = _context.Bookings.Where(b => b.IdGuest == user.Id);
            _context.Bookings.RemoveRange(bookings);

            var routes = _context.SavedRoutes.Where(r => r.IdGuest == user.Id);
            _context.SavedRoutes.RemoveRange(routes);

            var messages = _context.Messages.Where(b => b.IdSender == user.Id);
            _context.Messages.RemoveRange(messages);

            var messages2 = _context.Messages.Where(b => b.IdReceiver == user.Id);
            _context.Messages.RemoveRange(messages2);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShelter(int id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null)
            {
                return NotFound();
            }

            var reviews = _context.Reviews.Where(r => r.IdShelter == shelter.Id);
            _context.Reviews.RemoveRange(reviews);

            var bookings = _context.Bookings.Where(b => b.IdShelter == shelter.Id);
            _context.Bookings.RemoveRange(bookings);

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmShelter(int id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null)
            {
                return NotFound();
            }

            shelter.ConfirmedShelter = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UnconfirmShelter(int id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null)
            {
                return NotFound();
            }

            shelter.ConfirmedShelter = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddTag([FromBody] TagAdminViewModel model)
        {
            var tag = new Tag { Name = model.Name };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddFacility([FromBody] FacilityAdminViewModel model)
        {
            var facility = new Facility { Name = model.Name };
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            _context.Facilities.Remove(facility);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
