using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SchroniskaTurystyczne.Controllers
{
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MapController(ApplicationDbContext context, UserManager<AppUser> userManager)
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
                if(point.IdShelter != null)
                {
                    var shelter = await _context.Shelters
                    .Where(r => r.Id == point.IdShelter)
                    .FirstOrDefaultAsync();

                    if (shelter != null)
                        point.Shelter = shelter;
                }
                _context.Points.Add(point);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MapView", "Map");
        }

        public IActionResult MapView(int id)
        {
            // Get the current user
            var currentUser = _userManager.GetUserAsync(User).Result;

            // Fetch shelters
            var shelters = _context.Shelters
                .Select(shelter => new
                {
                    id = shelter.Id,
                    idExhibitor = shelter.IdExhibitor,
                    name = shelter.Name,
                    rating = shelter.Rating,
                    confirmedShelter = shelter.ConfirmedShelter,
                    country = shelter.Country,
                    city = shelter.City,
                    street = shelter.Street,
                    locationLon = shelter.LocationLon,
                    locationLat = shelter.LocationLat,
                    category = shelter.Category.Name, // Fetch category name
                    tags = string.Join(", ", shelter.Tags.Select(tag => tag.Name)) // Fetch tag names as a comma-separated string
                })
                .ToList();

            // Fetch user's routes with limited data to avoid circular references
            var userRoutes = _context.SavedRoutes
                .Where(r => r.IdGuest == currentUser.Id)
                .Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Points = r.Points
                        .OrderBy(p => p.Number)
                        .Select(p => new
                        {
                            Id = p.Id,
                            LocationLon = p.LocationLon,
                            LocationLat = p.LocationLat,
                            Number = p.Number,
                            ShelterId = p.IdShelter // Include shelter ID if available
                        })
                        .ToList()
                })
                .ToList();

            Debug.WriteLine(userRoutes);

            // Pass the selected shelter ID if needed
            ViewBag.SelectedShelterId = id;

            // Serialize routes to be used in JavaScript
            ViewBag.UserRoutes = System.Text.Json.JsonSerializer.Serialize(userRoutes);

            return View(shelters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var route = await _context.SavedRoutes
                .FirstOrDefaultAsync(r => r.Id == id && r.IdGuest == currentUser.Id);

            if (route == null)
            {
                return NotFound();
            }

            // Remove associated points first
            var points = _context.Points.Where(p => p.IdSavedRoute == id);
            _context.Points.RemoveRange(points);

            // Remove the route
            _context.SavedRoutes.Remove(route);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetUserRoutes()
        {
            var userId = _userManager.GetUserId(User);
            var routes = _context.SavedRoutes
                .Where(r => r.IdGuest == userId)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Points = r.Points
                        .OrderBy(p => p.Number)
                        .Select(p => new
                        {
                            p.LocationLon,
                            p.LocationLat,
                            Shelter = p.Shelter != null ? new
                            {
                                p.Shelter.Id,
                                p.Shelter.Name,
                                LocationLat = p.Shelter.LocationLat,
                                LocationLon = p.Shelter.LocationLon
                            } : null
                        })
                        .ToList()
                })
                .ToList();

            return Json(routes);
        }

        [HttpGet("Map/GetShelterRoutes/{shelterId}")]
        public IActionResult GetShelterRoutes(int shelterId)
        {
            Debug.WriteLine(shelterId);
            var userId = _userManager.GetUserId(User);
            var routes = _context.SavedRoutes
                .Where(r => r.Points.Any(p => p.IdShelter == shelterId))
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Points = r.Points
                        .OrderBy(p => p.Number)
                        .Select(p => new
                        {
                            p.LocationLon,
                            p.LocationLat,
                            Shelter = p.Shelter != null ? new
                            {
                                p.Shelter.Id,
                                p.Shelter.Name,
                                LocationLat = p.Shelter.LocationLat,
                                LocationLon = p.Shelter.LocationLon
                            } : null
                        })
                        .ToList()
                })
                .ToList();

            Debug.WriteLine(routes);

            return Json(routes);
        }

        [HttpGet("Map/GetRouteDetails/{routeId}")]
        public IActionResult GetRouteDetails(int routeId)
        {
            var route = _context.SavedRoutes
                .Where(r => r.Id == routeId)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Points = r.Points
                        .OrderBy(p => p.Number)
                        .Select(p => new
                        {
                            p.LocationLon,
                            p.LocationLat,
                            Shelter = p.Shelter != null ? new
                            {
                                p.Shelter.Id,
                                p.Shelter.Name,
                                LocationLat = p.Shelter.LocationLat,
                                LocationLon = p.Shelter.LocationLon
                            } : null
                        })
                        .ToList()
                })
                .FirstOrDefault();

            return Json(route);
        }
    }
}
