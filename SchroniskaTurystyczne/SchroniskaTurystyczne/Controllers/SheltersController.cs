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

        public SheltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shelters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shelters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Rating,LocationLon,LocationLat")] Shelter shelter)
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (ModelState.IsValid)
            //{
            shelter.IdExhibitor = "e2d9b874-62a8-455a-9186-40c10b091d02";

                _context.Add(shelter);
                await _context.SaveChangesAsync();
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
                    Exhibitor = s.Exhibitor.FirstName + " " + s.Exhibitor.LastName, // Dane o wystawiającym
                })
                .ToList();

            // Przekaż schroniska do widoku
            ViewBag.Shelters = shelters;

            foreach (var shelter in shelters)
            {
                Console.WriteLine($"Shelter: {shelter.Name}, Lat: {shelter.LocationLat}, Lon: {shelter.LocationLon}");
            }

            return View("~/Views/Routes/Create.cshtml");
        }
    }
}
