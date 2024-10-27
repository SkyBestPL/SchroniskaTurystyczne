using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SchroniskaTurystyczne.Controllers
{
    public class SheltersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SheltersController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Shelters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shelters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,LocationLon,LocationLat")] Shelter shelter)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            shelter.IdExhibitor = user.Id;
            shelter.Exhibitor = user;

            //if (ModelState.IsValid)
            //{
                _context.Add(shelter);
                await _context.SaveChangesAsync();

            // Przekierowanie do formularza dodawania pokoju po utworzeniu schroniska
                return RedirectToAction(nameof(Index));
            //}

            //return View(shelter);
        }

        // GET: Shelters
        public async Task<IActionResult> Index()
        {
            var shelters = await _context.Shelters
                .Include(s => s.Rooms)
                .Include(s => s.Photos)
                .Include(s => s.Exhibitor)
                .ToListAsync();
            return View(shelters);
        }

        public IActionResult CreateRoute()
        {
            var shelters = _context.Shelters
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.LocationLat,
                    s.LocationLon,
                    s.Rating,
                    Exhibitor = s.Exhibitor.FirstName + " " + s.Exhibitor.LastName,
                })
                .ToList();

            ViewBag.Shelters = shelters;

            foreach (var shelter in shelters)
            {
                Console.WriteLine($"Shelter: {shelter.Name}, Lat: {shelter.LocationLat}, Lon: {shelter.LocationLon}");
            }

            return View("~/Views/Routes/Create.cshtml");
        }
    }
}
