using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using System.Security.Claims;

namespace SchroniskaTurystyczne.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            var shelter = await _context.Shelters
                .Include(s => s.Rooms)
                    .ThenInclude(r => r.BookingRooms)
                .Include(s => s.Photos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
            {
                return NotFound();
            }

            shelter.Rooms = shelter.Rooms.Where(r => r.IsActive).ToList();

            return View(shelter);
        }

        public IActionResult GetRoomBookings(int roomId)
        {
            // Pobierz wszystkie rezerwacje dla danego pokoju
            var bookings = _context.BookingRooms
                .Where(br => br.IdRoom == roomId)
                .Select(br => new
                {
                    CheckInDate = br.Booking.CheckInDate,
                    CheckOutDate = br.Booking.CheckOutDate,
                    NumberOfPeople = br.NumberOfPeople
                })
                .ToList();

            // Pobierz pojemność pokoju
            var roomCapacity = _context.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => r.Capacity)
                .FirstOrDefault();

            // Lista dni zliczająca liczbę osób dla każdego dnia w danym zakresie rezerwacji
            var dailyOccupancy = new Dictionary<DateTime, int>();

            // Przejdź przez każdą rezerwację i podziel ją na poszczególne dni
            foreach (var booking in bookings)
            {
                for (var date = booking.CheckInDate.Date; date < booking.CheckOutDate.Date; date = date.AddDays(1))
                {
                    if (dailyOccupancy.ContainsKey(date))
                    {
                        dailyOccupancy[date] += booking.NumberOfPeople;
                    }
                    else
                    {
                        dailyOccupancy[date] = booking.NumberOfPeople;
                    }
                }
            }

            // Przetwórz dane do formatu wymagającego przez FullCalendar
            var events = dailyOccupancy.Select(d => new
            {
                start = d.Key.ToString("yyyy-MM-dd"),
                end = d.Key.AddDays(1).ToString("yyyy-MM-dd"),
                color = (d.Value >= roomCapacity) ? "#ff6969" : "#fdff69",
                title = $"{d.Value}/{roomCapacity}",
                textColor = "#000000"
            }).ToList();

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest request)
        {
            var checkInDate = request.CheckInDate;
            var checkOutDate = request.CheckOutDate;
            var rooms = request.Rooms;

            foreach (var room in rooms)
            {
                var roomBookings = _context.BookingRooms
                    .Where(br => br.IdRoom == room.RoomId &&
                                 br.Booking.CheckOutDate > checkInDate &&
                                 br.Booking.CheckInDate < checkOutDate)
                    .Select(br => new
                    {
                        CheckInDate = br.Booking.CheckInDate,
                        CheckOutDate = br.Booking.CheckOutDate,
                        NumberOfPeople = br.NumberOfPeople
                    }).ToList();

                int totalOccupancy = 0;
                foreach (var b in roomBookings)
                {
                    totalOccupancy += b.NumberOfPeople;
                    if (totalOccupancy + room.NumberOfPeople > _context.Rooms
                            .Where(r => r.Id == room.RoomId)
                            .Select(r => r.Capacity)
                            .FirstOrDefault())
                    {
                        return Json(new { success = false, message = "Przekroczona pojemność pokoju w wybranym okresie." });
                    }
                }
            }

            // Dodaj nową rezerwację do bazy
            var booking = new Booking
            {
                IdGuest = User.FindFirstValue(ClaimTypes.NameIdentifier),
                NumberOfPeople = rooms.Sum(r => r.NumberOfPeople),
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                BookingDate = DateTime.UtcNow,
                Bill = rooms.Sum(r => r.NumberOfPeople * _context.Rooms.Where(ro => ro.Id == r.RoomId).Select(ro => ro.PricePerNight).FirstOrDefault() * (checkOutDate - checkInDate).Days),
                Paid = false,
                Verified = false
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var room in rooms)
            {
                var bookingRoom = new BookingRoom
                {
                    IdBooking = booking.Id,
                    IdRoom = room.RoomId,
                    NumberOfPeople = room.NumberOfPeople
                };
                _context.BookingRooms.Add(bookingRoom);
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                    .ThenInclude(br => br.Room)
                        .ThenInclude(r => r.Shelter)
                            .ThenInclude(s => s.Photos)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Sprawdź czy użytkownik jest właścicielem rezerwacji
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.IdGuest != userId)
            {
                return Forbid();
            }

            // Sprawdź czy rezerwacja może być edytowana (nie jest potwierdzona)
            if (booking.Verified || booking.Paid)
            {
                return RedirectToAction("Index", "Home");
            }

            var shelter = booking.BookingRooms.First().Room.Shelter;
            var viewModel = new BookingEditViewModel
            {
                BookingId = booking.Id,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                Shelter = shelter,
                SelectedRooms = booking.BookingRooms.ToDictionary(
                    br => br.IdRoom,
                    br => br.NumberOfPeople
                )
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId);

            if (booking == null)
            {
                return Json(new { success = false, message = "Rezerwacja nie istnieje." });
            }

            // Sprawdź prawa dostępu
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.IdGuest != userId)
            {
                return Forbid();
            }

            if (booking.Verified)
            {
                return Json(new { success = false, message = "Nie można edytować potwierdzonej rezerwacji." });
            }

            // Sprawdź dostępność pokoi
            foreach (var room in request.Rooms)
            {
                var roomBookings = await _context.BookingRooms
                    .Where(br => br.IdRoom == room.RoomId &&
                                br.Booking.Id != booking.Id && // Wykluczamy obecną rezerwację
                                br.Booking.CheckOutDate > request.CheckInDate &&
                                br.Booking.CheckInDate < request.CheckOutDate)
                    .Select(br => new
                    {
                        br.NumberOfPeople,
                        RoomCapacity = br.Room.Capacity
                    })
                    .ToListAsync();

                int totalOccupancy = roomBookings.Sum(rb => rb.NumberOfPeople);
                var roomCapacity = await _context.Rooms
                    .Where(r => r.Id == room.RoomId)
                    .Select(r => r.Capacity)
                    .FirstOrDefaultAsync();

                if (totalOccupancy + room.NumberOfPeople > roomCapacity)
                {
                    return Json(new { success = false, message = "Przekroczona pojemność pokoju w wybranym okresie." });
                }
            }

            // Aktualizuj rezerwację
            booking.CheckInDate = request.CheckInDate;
            booking.CheckOutDate = request.CheckOutDate;
            booking.NumberOfPeople = request.Rooms.Sum(r => r.NumberOfPeople);
            booking.BookingDate = DateTime.UtcNow;

            // Usuń stare BookingRooms
            _context.BookingRooms.RemoveRange(booking.BookingRooms);

            // Dodaj nowe BookingRooms
            foreach (var room in request.Rooms)
            {
                var bookingRoom = new BookingRoom
                {
                    IdBooking = booking.Id,
                    IdRoom = room.RoomId,
                    NumberOfPeople = room.NumberOfPeople
                };
                _context.BookingRooms.Add(bookingRoom);
            }

            // Przelicz cenę
            booking.Bill = await CalculateBookingPrice(request.Rooms, request.CheckInDate, request.CheckOutDate);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private async Task<double> CalculateBookingPrice(List<RoomBookingRequest> rooms, DateTime checkIn, DateTime checkOut)
        {
            double totalPrice = 0;
            int numberOfDays = (checkOut - checkIn).Days;

            foreach (var room in rooms)
            {
                var roomPrice = await _context.Rooms
                    .Where(r => r.Id == room.RoomId)
                    .Select(r => r.PricePerNight)
                    .FirstOrDefaultAsync();

                totalPrice += roomPrice * room.NumberOfPeople * numberOfDays;
            }

            return totalPrice;
        }
    }
}
