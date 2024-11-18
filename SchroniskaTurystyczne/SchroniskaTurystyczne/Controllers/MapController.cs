using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;

namespace SchroniskaTurystyczne.Controllers
{
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult MapView(int id)
        {
            var shelters = _context.Shelters.ToList();
            ViewBag.SelectedShelterId = id;
            return View(shelters);
        }

        public async Task<IActionResult> GetUserRoutes(string userId)
        {
            var routes = await _context.SavedRoutes
                .Where(route => route.IdGuest == userId)
                .Include(route => route.Points)
                .ToListAsync();

            return Json(routes);
        }
    }
}
