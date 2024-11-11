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

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
