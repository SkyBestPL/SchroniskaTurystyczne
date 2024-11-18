using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Bind("Name,Description,Country,City,Street,StreetNumber,ZipCode,LocationLon,LocationLat,Rooms")] Shelter shelter, string SelectedTags, IFormFileCollection Photos)
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
                    room.HasConfirmedBooking = false;

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

        // GET: Shelters/Edit/5
        [Authorize(Policy = "RequireExhibitorRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shelter = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.Facilities)
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(s => s.Tags)
                .Include(s => s.Photos)
                .Include(s => s.Exhibitor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
            {
                return NotFound();
            }

            // Sprawdź czy zalogowany użytkownik jest właścicielem schroniska
            var currentUser = await _userManager.GetUserAsync(User);
            if (shelter.IdExhibitor != currentUser.Id)
            {
                return Forbid();
            }

            // Przygotuj dane dla widoku
            ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Facilities = await _context.Facilities.ToListAsync();

            // Przygotuj string z ID wybranych tagów
            ViewBag.SelectedTags = string.Join(",", shelter.Tags.Select(t => t.Id));

            // Przygotuj string z ID wybranych udogodnień dla każdego pokoju
            foreach (var room in shelter.Rooms)
            {
                room.SelectedFacilities = string.Join(",", room.Facilities.Select(f => f.Id));
            }

            return View(shelter);
        }

        // POST: Shelters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireExhibitorRole")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Country,City,Street,StreetNumber,ZipCode,LocationLon,LocationLat,Rooms")] Shelter shelter,
            string SelectedTags, IFormFileCollection NewPhotos, List<int> PhotosToDelete)
        {
            if (id != shelter.Id)
            {
                return NotFound();
            }

            var existingShelter = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.Facilities)
                .Include(s => s.Tags)
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShelter == null)
            {
                return NotFound();
            }

            // Sprawdź czy zalogowany użytkownik jest właścicielem schroniska
            var currentUser = await _userManager.GetUserAsync(User);
            if (existingShelter.IdExhibitor != currentUser.Id)
            {
                return Forbid();
            }

            try
            {
                // Aktualizuj podstawowe informacje
                existingShelter.Name = shelter.Name;
                existingShelter.Description = shelter.Description;
                existingShelter.Country = shelter.Country;
                existingShelter.City = shelter.City;
                existingShelter.Street = shelter.Street;
                existingShelter.StreetNumber = shelter.StreetNumber;
                existingShelter.ZipCode = shelter.ZipCode;
                existingShelter.LocationLon = shelter.LocationLon;
                existingShelter.LocationLat = shelter.LocationLat;

                // Aktualizuj tagi
                existingShelter.Tags.Clear();
                if (!string.IsNullOrEmpty(SelectedTags))
                {
                    var tagIds = SelectedTags.Split(',').Select(int.Parse).ToList();
                    var tags = await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                    foreach (var tag in tags)
                    {
                        existingShelter.Tags.Add(tag);
                    }
                }

                // Usuń zaznaczone zdjęcia
                if (PhotosToDelete != null && PhotosToDelete.Any())
                {
                    var photosToRemove = existingShelter.Photos.Where(p => PhotosToDelete.Contains(p.Id)).ToList();
                    foreach (var photo in photosToRemove)
                    {
                        existingShelter.Photos.Remove(photo);
                        _context.Photos.Remove(photo);
                    }
                }

                // Dodaj nowe zdjęcia
                foreach (var photoFile in NewPhotos)
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await photoFile.CopyToAsync(memoryStream);
                            if (memoryStream.Length < 10097152) // Limit 10 MB
                            {
                                var photo = new Photo
                                {
                                    Name = photoFile.FileName,
                                    PhotoData = memoryStream.ToArray(),
                                    Shelter = existingShelter
                                };
                                existingShelter.Photos.Add(photo);
                            }
                            else
                            {
                                ModelState.AddModelError("NewPhotos", "Jedno ze zdjęć jest za duże. Maksymalny rozmiar to 10 MB.");
                                return View(existingShelter);
                            }
                        }
                    }
                }

                // Usuń pokoje, których nie ma w nowej kolekcji
                var existingRoomIds = existingShelter.Rooms.Select(r => r.Id).ToList();
                var updatedRoomIds = shelter.Rooms?.Select(r => r.Id).ToList() ?? new List<int>();
                var roomsToDelete = existingShelter.Rooms.Where(r => !updatedRoomIds.Contains(r.Id)).ToList();

                foreach (var room in roomsToDelete)
                {
                    // Usuń rezerwacje związane z tym pokojem
                    var relatedBookings = _context.BookingRooms
                        .Where(br => br.IdRoom == room.Id)
                        .Select(br => br.Booking)
                        .Distinct()
                        .ToList();

                    foreach (var booking in relatedBookings)
                    {
                        // Usuń powiązane wpisy z BookingRoom
                        var bookingRooms = _context.BookingRooms.Where(br => br.IdBooking == booking.Id).ToList();
                        _context.BookingRooms.RemoveRange(bookingRooms);

                        // Jeśli po usunięciu nie ma innych powiązań, usuń samą rezerwację
                        //if (!bookingRooms.Any(br => br.IdBooking == booking.Id))
                        //{
                            _context.Bookings.Remove(booking);
                        //}
                    }

                    // Usuń pokój
                    _context.Rooms.Remove(room);
                }

                // Aktualizuj lub dodaj nowe pokoje
                if (shelter.Rooms != null)
                {
                    foreach (var room in shelter.Rooms)
                    {
                        if (room.Id == 0)
                        {
                            // Nowy pokój
                            room.Shelter = existingShelter;
                            existingShelter.Rooms.Add(room);
                        }
                        else
                        {
                            // Istniejący pokój
                            var existingRoom = existingShelter.Rooms.FirstOrDefault(r => r.Id == room.Id);
                            if (existingRoom != null)
                            {
                                existingRoom.Name = room.Name;
                                existingRoom.Description = room.Description;
                                existingRoom.PricePerNight = room.PricePerNight;
                                existingRoom.Capacity = room.Capacity;
                                existingRoom.IdType = room.IdType;
                                existingRoom.IsActive = room.IsActive;

                                // Aktualizuj udogodnienia
                                existingRoom.Facilities.Clear();
                                if (!string.IsNullOrEmpty(room.SelectedFacilities))
                                {
                                    var facilityIds = room.SelectedFacilities
                                        .Split(',')
                                        .Where(id => !string.IsNullOrWhiteSpace(id))
                                        .Select(int.Parse)
                                        .ToList();

                                    var facilities = await _context.Facilities
                                        .Where(f => facilityIds.Contains(f.Id))
                                        .ToListAsync();

                                    foreach (var facility in facilities)
                                    {
                                        existingRoom.Facilities.Add(facility);
                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Shelters");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShelterExists(shelter.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ShelterExists(int id)
        {
            return _context.Shelters.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> ManageBookings(int id)
        {
            var shelter = await _context.Shelters
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
                return NotFound();

            // Sprawdzenie czy zalogowany użytkownik jest właścicielem schroniska
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shelter.IdExhibitor != currentUserId)
                return Forbid();

            var bookings = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .Where(b => b.BookingRooms.Any(br => br.Room.IdShelter == id))
                .ToListAsync();

            var viewModel = new ManageBookingsViewModel
            {
                ShelterId = id,
                ShelterName = shelter.Name,
                PendingBookings = await GetBookingDetails(bookings.Where(b => !b.Verified)),
                ConfirmedBookings = await GetBookingDetails(bookings.Where(b => b.Verified && b.CheckOutDate > DateTime.Now)),
                CompletedBookings = await GetBookingDetails(bookings.Where(b => b.Verified && b.CheckOutDate <= DateTime.Now))
            };

            return View(viewModel);
        }

        private async Task<List<BookingDetailsViewModel>> GetBookingDetails(IEnumerable<Booking> bookings)
        {
            return bookings.Select(b => new BookingDetailsViewModel
            {
                BookingId = b.Id,
                GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalPrice = b.Bill,
                Rooms = b.BookingRooms.Select(br => new RoomBookingDetail
                {
                    RoomName = br.Room.Name ?? $"Pokój {br.Room.Id}",
                    NumberOfPeople = br.NumberOfPeople,
                    PricePerNight = br.Room.PricePerNight
                }).ToList()
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            // Sprawdzenie uprawnień
            var shelter = await _context.Rooms
                .Where(r => r.Id == booking.BookingRooms.First().IdRoom)
                .Select(r => r.Shelter)
                .FirstOrDefaultAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shelter.IdExhibitor != currentUserId)
                return Forbid();

            booking.Verified = true;
            foreach (var bookingRoom in booking.BookingRooms)
            {
                bookingRoom.Room.HasConfirmedBooking = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBookings), new { id = shelter.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RejectBooking(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            // Sprawdzenie uprawnień
            var shelter = await _context.Rooms
                .Where(r => r.Id == booking.BookingRooms.First().IdRoom)
                .Select(r => r.Shelter)
                .FirstOrDefaultAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shelter.IdExhibitor != currentUserId)
                return Forbid();

            _context.BookingRooms.RemoveRange(booking.BookingRooms);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings), new { id = shelter.Id });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteBooking(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            // Sprawdzenie uprawnień
            var shelter = await _context.Rooms
                .Where(r => r.Id == booking.BookingRooms.First().IdRoom)
                .Select(r => r.Shelter)
                .FirstOrDefaultAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shelter.IdExhibitor != currentUserId)
                return Forbid();

            _context.BookingRooms.RemoveRange(booking.BookingRooms);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings), new { id = shelter.Id });
        }
    }
}
