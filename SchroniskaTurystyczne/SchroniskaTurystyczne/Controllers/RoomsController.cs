using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;

namespace SchroniskaTurystyczne.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms/Create
        public IActionResult Create(int shelterId)
        {
            ViewBag.ShelterId = shelterId; // Przekazujemy ID schroniska do widoku
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IdType,ShelterId")] Room room)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(room);
                await _context.SaveChangesAsync();

                // Po dodaniu pokoju, przekieruj do edycji schroniska
                return RedirectToAction("Create", "Shelters", new { id = room.IdShelter });
            //}

            //ViewBag.ShelterId = room.IdShelter; // Jeśli dodanie się nie uda, wracamy do widoku z danymi schroniska
            //return View(room);
        }
    }
}