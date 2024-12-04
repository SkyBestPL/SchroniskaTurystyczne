using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.Controllers
{
    public class RoutesController : Controller
    {
        /*private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RoutesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Routes/Create
        public IActionResult Create()
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

            return View();
        }

        // POST: Routes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavedRoute route, string routePointsJson)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            route.IdGuest = user.Id;

            _context.SavedRoutes.Add(route);

            await _context.SaveChangesAsync();

            List<Point> routePoints = JsonConvert.DeserializeObject<List<Point>>(routePointsJson);

            int pointNumber = 1;

            foreach (var point in routePoints)
            {
                point.IdSavedRoute = route.Id;
                point.Number = pointNumber++;
                _context.Points.Add(point);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Shelters");
        }

        public async Task<IActionResult> UserRoutes()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var routes = await _context.SavedRoutes
                .Where(r => r.IdGuest == user.Id)
                .Include(r => r.Points)
                .ToListAsync();

            return View(routes);
        }*/
    }
}
