using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SchroniskaTurystyczne.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

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

        [Authorize(Policy = "RequireExhibitorRole")]
        public IActionResult Create()
        {
            var viewModel = new ShelterCreateViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),
                RoomTypes = _context.RoomTypes.Select(rt => new SelectListItem
                {
                    Value = rt.Id.ToString(),
                    Text = rt.Name
                }).ToList(),
                Tags = _context.Tags.ToList(),
                Facilities = _context.Facilities.ToList(),
                Rooms = new List<RoomViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShelterCreateViewModel model)
        {
            if (model.SelectedTags == null)
            {
                model.SelectedTags = new List<int>();
            }

            if (!ModelState.IsValid)
            {
                var errorMessages = new List<string>();

                foreach (var state in ModelState)
                {
                    string key = state.Key;
                    var errors = state.Value.Errors;

                    foreach (var error in errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            if (key.StartsWith("Rooms"))
                            {
                                if (key.Contains(".Name"))
                                {
                                    errorMessages.Add("Jeden lub więcej pokoi nie ma nazwy.");
                                }
                                else if (key.Contains(".Capacity"))
                                {
                                    errorMessages.Add("Jeden lub więcej pokoi nie ma poprawnie określonej ilości miejsc.");
                                }
                                else if (key.Contains(".PricePerNight"))
                                {
                                    errorMessages.Add("Jeden lub więcej pokoi nie ma poprawnie określonej ceny.");
                                }
                            }
                            else
                            {
                                errorMessages.Add("Przy tworzeniu schroniska wystąpiły błędy, należy ponownie uzupełnić formularz.");
                            }
                        }
                    }
                }

                errorMessages.Add("Uwaga: Jeżeli dodawane były jakieś zdjęcia, należy wgrać je ponownie.");
                errorMessages = errorMessages.Distinct().ToList();
                TempData["ErrorMessage"] = string.Join("<br>", errorMessages);

                model.SelectedTags ??= new List<int>();

                model.Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToListAsync();

                model.RoomTypes = await _context.RoomTypes
                    .Select(rt => new SelectListItem
                    {
                        Value = rt.Id.ToString(),
                        Text = rt.Name
                    }).ToListAsync();

                model.Tags = await _context.Tags.ToListAsync();
                model.Facilities = await _context.Facilities.ToListAsync();

                if (model.Rooms == null || model.Rooms.Count == 0)
                {
                    model.Rooms = new List<RoomViewModel>();
                }

                return View(model);
            }

            var photoErrors = CheckPhotos(model);

            if (photoErrors.Any())
            {
                model.SelectedTags ??= new List<int>();

                model.Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToListAsync();

                model.RoomTypes = await _context.RoomTypes
                    .Select(rt => new SelectListItem
                    {
                        Value = rt.Id.ToString(),
                        Text = rt.Name
                    }).ToListAsync();

                model.Tags = await _context.Tags.ToListAsync();
                model.Facilities = await _context.Facilities.ToListAsync();

                if (model.Rooms == null || model.Rooms.Count == 0)
                {
                    model.Rooms = new List<RoomViewModel>();
                }

                TempData["ErrorMessage"] = string.Join("<br>", photoErrors);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var shelter = new Shelter
            {
                Name = model.Name,
                Description = model.Description,
                Country = model.Country,
                City = model.City,
                Street = model.Street,
                StreetNumber = model.StreetNumber,
                ZipCode = model.ZipCode,
                LocationLat = model.LocationLat,
                LocationLon = model.LocationLon,
                IdCategory = (int)model.IdCategory,
                IdExhibitor = user.Id,
                ConfirmedShelter = false,
                Tags = await _context.Tags.Where(t => model.SelectedTags.Contains(t.Id)).ToListAsync(),
                Rooms = new List<Room>()
            };

            foreach (var room in model.Rooms)
            {
                var roomEntity = new Room
                {
                    IdType = room.IdType ?? 0,
                    Name = room.Name,
                    PricePerNight = (double)room.PricePerNight,
                    Capacity = room.Capacity,
                    IsActive = room.IsActive,
                    Facilities = room.SelectedFacilities?.Any() == true
                        ? _context.Facilities.Where(f => room.SelectedFacilities.Contains(f.Id)).ToList()
                        : new List<Facility>(),
                    RoomPhotos = new List<RoomPhoto>()
                };

                if (room.RoomPhotos != null && room.RoomPhotos.Any())
                {
                    if (room.RoomPhotos.Count > 3)
                    {
                        ModelState.AddModelError("", "Można dodać maksymalnie 3 zdjęcia na pokój.");
                        return View(model);
                    }

                    foreach (var photoFile in room.RoomPhotos)
                    {
                        if (photoFile.Length > 5242880)
                        {
                            ModelState.AddModelError("", "Każde zdjęcie może mieć maksymalnie 5 MB.");
                            return View(model);
                        }

                        using var memoryStream = new MemoryStream();
                        await photoFile.CopyToAsync(memoryStream);

                        roomEntity.RoomPhotos.Add(new RoomPhoto
                        {
                            Name = photoFile.FileName,
                            PhotoData = memoryStream.ToArray()
                        });
                    }
                }

                shelter.Rooms.Add(roomEntity);
            }

            if (model.Photos != null && model.Photos.Any())
            {
                if (shelter.Photos == null)
                {
                    shelter.Photos = new List<Photo>();
                }

                foreach (var photoFile in model.Photos)
                {
                    using var memoryStream = new MemoryStream();
                    await photoFile.CopyToAsync(memoryStream);

                    if (memoryStream.Length < 10097152)
                    {
                        shelter.Photos.Add(new Photo
                        {
                            Name = photoFile.FileName,
                            PhotoData = memoryStream.ToArray()
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("Photos", "Jedno ze zdjęć jest za duże. Maksymalny rozmiar to 10 MB.");
                        return View(model);
                    }
                }
            }

            user.IdShelter = shelter.Id;
            user.Shelter = shelter;

            _context.Add(shelter);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private List<string> CheckPhotos(ShelterCreateViewModel model)
        {
            var errorMessages = new List<string>();

            // Sprawdź zdjęcia schroniska
            if (model.Photos != null && model.Photos.Count > 5) // Przykład: limit 5 zdjęć
            {
                errorMessages.Add("Schronisko może mieć maksymalnie 5 zdjęć.");
            }

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (photo.Length > 5 * 1024 * 1024) // Przykład: limit 5 MB
                    {
                        errorMessages.Add($"Zdjęcie {photo.FileName} przekracza dopuszczalny rozmiar 5 MB.");
                    }
                }
            }

            // Sprawdź zdjęcia pokoi
            if (model.Rooms != null)
            {
                foreach (var room in model.Rooms)
                {
                    if (room.RoomPhotos != null && room.RoomPhotos.Count > 3) // Przykład: limit 3 zdjęcia na pokój
                    {
                        errorMessages.Add($"Pokój {room.Name} może mieć maksymalnie 3 zdjęcia.");
                    }

                    if (room.RoomPhotos != null)
                    {
                        foreach (var photo in room.RoomPhotos)
                        {
                            if (photo.Length > 5 * 1024 * 1024) // Przykład: limit 5 MB
                            {
                                errorMessages.Add($"Zdjęcie {photo.FileName} w pokoju {room.Name} przekracza dopuszczalny rozmiar 5 MB.");
                            }
                        }
                    }
                }
            }

            return errorMessages;
        }

        // GET: Shelters
        /*public async Task<IActionResult> Index()
        {
            var shelters = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.Facilities)
                .Include(s => s.Photos)
                .Include(s => s.Exhibitor)
                .Include(s => s.Tags)
                .ToListAsync();
            return View(shelters);
        }*/

        public IActionResult Index(ShelterSearchViewModel searchModel)
        {
            var query = _context.Shelters
                .Include(s => s.Tags)
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(s => s.Photos)
                .Include(s => s.Category)
                .AsQueryable();

            // Filtruj według kategorii
            if (searchModel.SelectedCategoryId.HasValue)
            {
                query = query.Where(s => s.IdCategory == searchModel.SelectedCategoryId.Value);
            }

            // Aplikowanie filtrów
            if (!string.IsNullOrEmpty(searchModel.SearchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchModel.SearchTerm) ||
                    s.City.Contains(searchModel.SearchTerm));
            }

            if (!string.IsNullOrEmpty(searchModel.City))
            {
                query = query.Where(s => s.City == searchModel.City);
            }

            if (!string.IsNullOrEmpty(searchModel.Street))
            {
                query = query.Where(s => s.Street.Contains(searchModel.Street));
            }

            if (searchModel.SelectedTagIds != null && searchModel.SelectedTagIds.Any())
            {
                query = query.Where(s =>
                    searchModel.SelectedTagIds.All(tagId =>
                        s.Tags.Any(t => t.Id == tagId)
                    )
                );
            }

            if (searchModel.SelectedRoomTypeIds != null && searchModel.SelectedRoomTypeIds.Any())
            {
                query = query.Where(s =>
                    searchModel.SelectedRoomTypeIds.All(roomTypeId =>
                        s.Rooms.Any(r => r.RoomType.Id == roomTypeId)
                    )
                );
            }

            // Mapowanie na ViewModele
            var shelterViewModels = query.Select(s => new ShelterViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Rating = s.Rating,
                Country = s.Country,
                City = s.City,
                Street = s.Street,
                StreetNumber = s.StreetNumber,
                LocationLon = s.LocationLon,
                LocationLat = s.LocationLat,
                AmountOfReviews = s.AmountOfReviews,
                Tags = s.Tags.Select(t => new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }),
                MainPhotoBase64 = s.Photos != null && s.Photos.Any()
                    ? Convert.ToBase64String(s.Photos.First().PhotoData)
                    : null
            }).ToList();

            var viewModel = new ShelterSearchViewModel
            {
                SearchTerm = searchModel.SearchTerm,
                City = searchModel.City,
                Street = searchModel.Street,
                SelectedTagIds = searchModel.SelectedTagIds,
                SelectedRoomTypeIds = searchModel.SelectedRoomTypeIds,
                SelectedCategoryId = searchModel.SelectedCategoryId,
                AvailableTags = _context.Tags.Select(t => new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),
                AvailableRoomTypes = _context.RoomTypes.Select(rt => new RoomTypeViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    Description = rt.Description
                }).ToList(),
                Shelters = shelterViewModels,
                AvailableCategories = _context.Categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
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
                .Where(b => b.BookingRooms.Any(br => br.Room.IdShelter == id) && b.Valid == true)
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

            booking.Verified = false;
            booking.Valid = false;
            booking.Ended = false;

            //_context.BookingRooms.RemoveRange(booking.BookingRooms);
            //_context.Bookings.Remove(booking);
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

            booking.Ended = true;

            //_context.BookingRooms.RemoveRange(booking.BookingRooms);
            //_context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings), new { id = shelter.Id });
        }

        public async Task<IActionResult> MyShelter()
        {
            var user = await _userManager.GetUserAsync(User);

            // Znajdź schronisko użytkownika
            var shelter = await _context.Shelters
                .Include(s => s.Rooms)
                .FirstOrDefaultAsync(s => s.IdExhibitor == user.Id);

            if (shelter == null)
            {
                return View(new MyShelterViewModel
                {
                    HasShelter = false
                });
            }

            // Statystyki rezerwacji
            var approvedBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && b.Verified)
                .CountAsync();

            var pendingBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && !b.Verified)
                .CountAsync();

            var endedBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && b.Ended)
                .CountAsync();

            return View(new MyShelterViewModel
            {
                HasShelter = true,
                Shelter = shelter,
                Rooms = shelter.Rooms.ToList(),
                BookingStatistics = new Dictionary<string, int>
            {
                { "Approved", approvedBookings },
                { "Pending", pendingBookings },
                { "Ended", endedBookings }
            }
            });
        }
    }
}
