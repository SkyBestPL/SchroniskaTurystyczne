using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SchroniskaTurystyczne.Controllers
{
    public class SheltersController : Controller
    {
        private readonly ApplicationDbContext _context;

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
            //if (ModelState.IsValid)
            //{
                shelter.IdExhibitor = 2; //tymczasowo wlasciciel ma zawsze id = 2

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
    }
}
