using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "RequireExhibitorRole")]
        public IActionResult Create()
        {
            ViewBag.RoomTypes = _context.RoomTypes.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.Facilities = _context.Facilities.ToList();
            return View();
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("Name,Description,Country,City,Street,StreetNumber,LocationLon,LocationLat,Rooms")] Shelter shelter, string SelectedTags, IFormFileCollection Photos)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            shelter.IdExhibitor = user.Id;
            shelter.Exhibitor = user;

            // Przypisz pokoje i ich udogodnienia do schroniska
            if (shelter.Rooms != null)
            {
                foreach (var room in shelter.Rooms)
                {
                    room.Shelter = shelter;

                    if (!string.IsNullOrEmpty(room.SelectedFacilities))
                    {
                        var facilityIds = room.SelectedFacilities
                            .Split(',')
                            .Where(id => !string.IsNullOrWhiteSpace(id))
                            .Select(int.Parse)
                            .ToList();

                        room.Facilities = await _context.Facilities
                            .Where(facility => facilityIds.Contains(facility.Id))
                            .ToListAsync();
                    }
                }
            }

            // Przypisz wybrane tagi do schroniska
            if (!string.IsNullOrEmpty(SelectedTags))
            {
                var tagIds = SelectedTags.Split(',').Select(int.Parse).ToList();
                shelter.Tags = await _context.Tags.Where(tag => tagIds.Contains(tag.Id)).ToListAsync();
            }

            shelter.Photos = new List<Photo>();
            foreach (var photoFile in Photos)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photoFile.CopyToAsync(memoryStream);

                        if (memoryStream.Length < 10097152) // Limit 10 MB na zdjęcie
                        {
                            var shelterPhoto = new Photo
                            {
                                IdShelter = shelter.Id,
                                Name = photoFile.FileName,
                                PhotoData = memoryStream.ToArray(),
                                Shelter = shelter
                            };

                            shelter.Photos.Add(shelterPhoto);
                        }
                        else
                        {
                            ModelState.AddModelError("Photos", "Jedno ze zdjęć jest za duże. Maksymalny rozmiar to 10 MB.");
                            return View(shelter);
                        }
                    }
                }
            }

            _context.Add(shelter);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Shelters
        public async Task<IActionResult> Index()
        {
            var shelters = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.Facilities)
                .Include(s => s.Photos)
                .Include(s => s.Exhibitor)
                .Include(s => s.Tags)
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

        public async Task<IActionResult> Booking(int id)
        {
            var shelter = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.BookingRooms)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Booking", new { id = id });
        }

        public async Task<IActionResult> GetRoomBookings(int roomId)
        {
            var bookings = await _context.BookingRooms
                .Where(br => br.IdRoom == roomId)
                .Select(br => new {
                    start = br.Booking.CheckInDate,
                    end = br.Booking.CheckOutDate,
                    title = "Zarezerwowane",
                    backgroundColor = "red"
                })
                .ToListAsync();

            return Json(bookings);
        }

        public IActionResult MapView(int id)
        {
            return RedirectToAction("MapView", "Map", new { id });
        }
    }
}
