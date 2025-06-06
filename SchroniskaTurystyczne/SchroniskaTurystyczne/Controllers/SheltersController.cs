﻿using Microsoft.AspNetCore.Mvc;
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

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

                photoErrors.Add("Uwaga: Wszystkie wgrane wcześniej zdjęcia należy załadować ponownie.");
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
                            PhotoData = CompressImage(memoryStream.ToArray()),
                            ThumbnailData = ResizeImage(memoryStream.ToArray(), 200)
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

                    if (memoryStream.Length < 5242880)
                    {
                        shelter.Photos.Add(new Photo
                        {
                            Name = photoFile.FileName,
                            PhotoData = CompressImage(memoryStream.ToArray()),
                            ThumbnailData = ResizeImage(memoryStream.ToArray(), 500)
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("Photos", "Jedno ze zdjęć jest za duże. Maksymalny rozmiar to 5 MB.");
                        return View(model);
                    }
                }
            }

            _context.Add(shelter);
            await _context.SaveChangesAsync();

            var generatedShelterId = shelter.Id;

            user.IdShelter = generatedShelterId;
            user.Shelter = shelter;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private byte[] ResizeImage(byte[] originalImage, int dimension)
        {
            using (var ms = new MemoryStream(originalImage))
            using (var originalBitmap = Image.FromStream(ms))
            {
                int newWidth, newHeight;

                if (originalBitmap.Width > originalBitmap.Height)
                {
                    newWidth = dimension;
                    newHeight = (int)(originalBitmap.Height * (dimension / (double)originalBitmap.Width));
                }
                else
                {
                    newHeight = dimension;
                    newWidth = (int)(originalBitmap.Width * (dimension / (double)originalBitmap.Height));
                }

                using (var resizedBitmap = new Bitmap(newWidth, newHeight))
                {
                    using (var graphics = Graphics.FromImage(resizedBitmap))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.Low;
                        graphics.CompositingMode = CompositingMode.SourceCopy;

                        graphics.DrawImage(
                            originalBitmap,
                            0, 0,
                            newWidth,
                            newHeight
                        );
                    }

                    using (var outputMs = new MemoryStream())
                    {
                        resizedBitmap.Save(outputMs, ImageFormat.Jpeg);
                        return outputMs.ToArray();
                    }
                }
            }
        }

        private byte[] CompressImage(byte[] originalImage)
        {
            long quality = 100;

            if (originalImage.Length > 2_500_000)
            {
                quality = 25;
            }
            else if (originalImage.Length > 1_000_000)
            {
                quality = 50;
            }
            else
            {
                quality = 80;
            }

            using (var ms = new MemoryStream(originalImage))
            using (var originalBitmap = Image.FromStream(ms))
            {
                using (var outputMs = new MemoryStream())
                {
                    var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                                                     .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpegEncoder == null)
                        throw new InvalidOperationException("JPEG encoder not found.");

                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    originalBitmap.Save(outputMs, jpegEncoder, encoderParams);

                    return outputMs.ToArray();
                }
            }
        }

        private List<string> CheckPhotos(ShelterCreateViewModel model)
        {
            var errorMessages = new List<string>();

            if (model.Photos != null && model.Photos.Count > 5)
            {
                errorMessages.Add("Schronisko może mieć maksymalnie 5 zdjęć.");
            }

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (photo.Length > 5 * 1024 * 1024)
                    {
                        errorMessages.Add($"Zdjęcie {photo.FileName} przekracza dopuszczalny rozmiar 5 MB.");
                    }
                }
            }

            if (model.Rooms != null)
            {
                foreach (var room in model.Rooms)
                {
                    if (room.RoomPhotos != null && room.RoomPhotos.Count > 3)
                    {
                        errorMessages.Add($"Pokój {room.Name} może mieć maksymalnie 3 zdjęcia.");
                    }

                    if (room.RoomPhotos != null)
                    {
                        foreach (var photo in room.RoomPhotos)
                        {
                            if (photo.Length > 5 * 1024 * 1024)
                            {
                                errorMessages.Add($"Zdjęcie {photo.FileName} w pokoju {room.Name} przekracza dopuszczalny rozmiar 5 MB.");
                            }
                        }
                    }
                }
            }

            return errorMessages;
        }

        public IActionResult Index(ShelterSearchViewModel searchModel)
        {
            var query = _context.Shelters
                .Include(s => s.Tags)
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(s => s.Photos)
                .Include(s => s.Category)
                .AsQueryable();

            if (searchModel.SelectedCategoryId.HasValue)
            {
                query = query.Where(s => s.IdCategory == searchModel.SelectedCategoryId.Value);
            }

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

            var shelterViewModels = query.Select(s => new ShelterViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Confirmed = s.ConfirmedShelter,
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
                    ? Convert.ToBase64String(s.Photos.First().ThumbnailData)
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
                .Select(br => new
                {
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
                .Include(s => s.Exhibitor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (shelter.IdExhibitor != currentUser.Id)
            {
                return Forbid();
            }

            ViewBag.RoomTypes = await _context.RoomTypes.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Facilities = await _context.Facilities.ToListAsync();

            ViewBag.SelectedTags = string.Join(",", shelter.Tags.Select(t => t.Id));

            foreach (var room in shelter.Rooms)
            {
                room.SelectedFacilities = string.Join(",", room.Facilities.Select(f => f.Id));
            }

            return View(shelter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireExhibitorRole")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Country,City,Street,StreetNumber,ZipCode,LocationLon,LocationLat,Rooms")] Shelter shelter, string SelectedTags)
        {
            if (id != shelter.Id)
            {
                return NotFound();
            }

            var existingShelter = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.Facilities)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShelter == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (existingShelter.IdExhibitor != currentUser.Id)
            {
                return Forbid();
            }

            try
            {
                existingShelter.Name = shelter.Name;
                existingShelter.Description = shelter.Description;
                existingShelter.Country = shelter.Country;
                existingShelter.City = shelter.City;
                existingShelter.Street = shelter.Street;
                existingShelter.StreetNumber = shelter.StreetNumber;
                existingShelter.ZipCode = shelter.ZipCode;
                existingShelter.LocationLon = shelter.LocationLon;
                existingShelter.LocationLat = shelter.LocationLat;

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

                var existingRoomIds = existingShelter.Rooms.Select(r => r.Id).ToList();
                var updatedRoomIds = shelter.Rooms?.Select(r => r.Id).ToList() ?? new List<int>();
                var roomsToDelete = existingShelter.Rooms.Where(r => !updatedRoomIds.Contains(r.Id)).ToList();

                foreach (var room in roomsToDelete)
                {
                    var relatedBookings = _context.BookingRooms
                        .Where(br => br.IdRoom == room.Id)
                        .Select(br => br.Booking)
                        .Distinct()
                        .ToList();

                    foreach (var booking in relatedBookings)
                    {
                        var bookingRooms = _context.BookingRooms.Where(br => br.IdBooking == booking.Id).ToList();
                        _context.BookingRooms.RemoveRange(bookingRooms);
                    }

                    _context.Rooms.Remove(room);
                }

                if (shelter.Rooms != null)
                {
                    foreach (var room in shelter.Rooms)
                    {
                        if (room.Id == 0)
                        {
                            room.Shelter = existingShelter;
                            existingShelter.Rooms.Add(room);
                        }
                        else
                        {
                            var existingRoom = existingShelter.Rooms.FirstOrDefault(r => r.Id == room.Id);
                            if (existingRoom != null)
                            {
                                existingRoom.Name = room.Name;
                                existingRoom.Description = room.Description;
                                existingRoom.PricePerNight = room.PricePerNight;
                                existingRoom.Capacity = room.Capacity;
                                existingRoom.IdType = room.IdType;
                                existingRoom.IsActive = room.IsActive;

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
                return RedirectToAction("MyShelter", "Shelters", new { id = shelter.Id });
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
                PendingBookings = await GetBookingDetails(bookings.Where(b => !b.Verified && !b.Ended && b.Valid)),
                ConfirmedBookings = await GetBookingDetails(bookings.Where(b => b.Verified && !b.Ended && b.Valid)),
                CompletedBookings = await GetBookingDetails(bookings.Where(b => b.Verified && b.Ended && b.Valid))
            };

            return View(viewModel);
        }

        private async Task<List<BookingDetailsViewModel>> GetBookingDetails(IEnumerable<Booking> bookings)
        {
            return bookings.Select(b => new BookingDetailsViewModel
            {
                BookingId = b.Id,
                GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                IdUser = b.IdGuest,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalPrice = b.Bill,
                Paid = b.Paid,
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

            var shelter = await _context.Rooms
                .Where(r => r.Id == booking.BookingRooms.First().IdRoom)
                .Select(r => r.Shelter)
                .FirstOrDefaultAsync();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (shelter.IdExhibitor != currentUserId)
                return Forbid();

            booking.Ended = true;

            foreach (var bookingRoom in booking.BookingRooms)
            {
                var hasActiveBookings = await _context.Bookings
                    .Include(b => b.BookingRooms)
                    .Where(b => b.Verified && b.Valid && !b.Ended && b.BookingRooms.Any(br => br.IdRoom == bookingRoom.Room.Id))
                    .AnyAsync();

                if (!hasActiveBookings)
                {
                    bookingRoom.Room.HasConfirmedBooking = false;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings), new { id = shelter.Id });
        }

        public async Task<IActionResult> MyShelter()
        {
            var user = await _userManager.GetUserAsync(User);

            var shelter = await _context.Shelters
                .Include(s => s.Rooms)
                .Include(s => s.Category)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.IdExhibitor == user.Id);

            if (shelter == null)
            {
                return View(new MyShelterViewModel
                {
                    HasShelter = false
                });
            }

            var approvedBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && b.Verified && !b.Ended && b.Valid)
                .CountAsync();

            var pendingBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && !b.Verified && !b.Ended && b.Valid)
                .CountAsync();

            var endedBookings = await _context.Bookings
                .Where(b => b.IdShelter == shelter.Id && b.Verified && b.Ended && b.Valid)
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

        [HttpGet]
        public async Task<IActionResult> EditRoomPhotos(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomPhotos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var viewModel = new RoomPhotosViewModel
            {
                RoomId = room.Id,
                RoomName = room.Name,
                Photos = room.RoomPhotos.Select(p => new RoomPhotoViewModel
                {
                    Id = p.Id,
                    ThumbnailData = p.ThumbnailData
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoomPhoto(int id)
        {
            var photo = await _context.RoomPhotos.FindAsync(id);

            if (photo == null)
            {
                return NotFound();
            }

            _context.RoomPhotos.Remove(photo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditRoomPhotos), new { id = photo.IdRoom });
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomPhoto(int roomId, IFormFile photo)
        {
            var currentPhotosCount = await _context.RoomPhotos.CountAsync(p => p.IdRoom == roomId);
            if (currentPhotosCount >= 3)
            {
                TempData["Error"] = "Nie można dodać więcej niż 3 zdjęcia dla jednego pokoju.";
                return RedirectToAction(nameof(EditRoomPhotos), new { id = roomId });
            }

            if (photo.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "Maksymalny rozmiar zdjęcia to 5MB.";
                return RedirectToAction(nameof(EditRoomPhotos), new { id = roomId });
            }

            if (photo != null && photo.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await photo.CopyToAsync(memoryStream);
                var newPhoto = new RoomPhoto
                {
                    IdRoom = roomId,
                    Name = photo.FileName,
                    ThumbnailData = ResizeImage(memoryStream.ToArray(), 200),
                    PhotoData = memoryStream.ToArray()
                };
                _context.RoomPhotos.Add(newPhoto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EditRoomPhotos), new { id = roomId });
        }

        [HttpGet]
        public IActionResult ViewFullRoomImage(int id)
        {
            var photo = _context.RoomPhotos.FirstOrDefault(p => p.Id == id);
            if (photo == null)
                return NotFound();

            return File(photo.PhotoData, "image/jpeg");
        }

        [HttpGet]
        public async Task<IActionResult> EditShelterPhotos(int id)
        {
            var shelter = await _context.Shelters
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
                return NotFound();

            var viewModel = new ShelterPhotosViewModel
            {
                ShelterId = shelter.Id,
                ShelterName = shelter.Name,
                Photos = shelter.Photos?.Select(p => new PhotoViewModel
                {
                    Id = p.Id,
                    ThumbnailData = p.ThumbnailData,
                    PhotoData = p.PhotoData
                }).ToList() ?? new List<PhotoViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddShelterPhoto(int shelterId, IFormFile photo)
        {
            var currentPhotosCount = await _context.Photos.CountAsync(p => p.IdShelter == shelterId);

            if (currentPhotosCount >= 5)
            {
                TempData["Error"] = "Nie można dodać więcej niż 5 zdjęć dla jednego schroniska.";
                return RedirectToAction(nameof(EditShelterPhotos), new { id = shelterId });
            }

            if (photo != null)
            {
                if (photo.Length > 10 * 1024 * 1024)
                {
                    TempData["Error"] = "Maksymalny rozmiar zdjęcia to 10MB.";
                    return RedirectToAction(nameof(EditShelterPhotos), new { id = shelterId });
                }

                using var memoryStream = new MemoryStream();
                await photo.CopyToAsync(memoryStream);
                var newPhoto = new Photo
                {
                    IdShelter = shelterId,
                    Name = photo.FileName,
                    ThumbnailData = ResizeImage(memoryStream.ToArray(), 500),
                    PhotoData = memoryStream.ToArray()
                };

                _context.Photos.Add(newPhoto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(EditShelterPhotos), new { id = shelterId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShelterPhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
                return NotFound();

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditShelterPhotos), new { id = photo.IdShelter });
        }

        [HttpGet]
        public async Task<IActionResult> ViewFullImage(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
                return NotFound();

            return File(photo.PhotoData, "image/jpeg");
        }
    }
}
