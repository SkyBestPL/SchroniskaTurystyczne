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
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null)
            {
                return NotFound();
            }

            return View(shelter);
        }

        /* [HttpGet]
         public async Task<IActionResult> GetRoomBookings(int roomId)
         {
             var bookings = await _context.BookingRooms
                 .Where(br => br.IdRoom == roomId)
                 .Select(br => new {
                     CheckInDate = br.Booking.CheckInDate.ToString("yyyy-MM-dd"),
                     CheckOutDate = br.Booking.CheckOutDate.ToString("yyyy-MM-dd")
                 })
                 .ToListAsync();

             return Json(bookings);
         }*/

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
                end = d.Key.AddDays(1).ToString("yyyy-MM-dd"), // FullCalendar wymaga, aby end był następnego dnia
                color = (d.Value >= roomCapacity) ? "#ff6969" : "#fdff69",
                title = $"{d.Value}/{roomCapacity}", // Informacja o liczbie osób i pojemności pokoju
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

        /*[HttpPost]
        public IActionResult GetBookings([FromBody] DateTimeRangeModel dateRange)
        {
            var bookings = _context.Bookings
                .Include(b => b.BookingRooms)
                .ThenInclude(br => br.Room)
                .Where(b => b.CheckInDate < dateRange.End && b.CheckOutDate > dateRange.Start)
                .ToList();

            var events = bookings.SelectMany(b => b.BookingRooms.GroupBy(br => br.Room.Id).Select(brGroup =>
            {
                var room = brGroup.First().Room;
                var totalPeople = brGroup.Sum(br => br.NumberOfPeople);

                var color = room.IdType == 2
                    ? "red" // prywatne pokoje
                    : (totalPeople <= room.Capacity ? "yellow" : null); // żółte jeśli nie przekracza pojemności

                return new
                {
                    roomId = room.Id,
                    roomName = room.Name,
                    idType = room.IdType,
                    checkInDate = b.CheckInDate.ToString("yyyy-MM-dd"),
                    checkOutDate = b.CheckOutDate.ToString("yyyy-MM-dd"),
                    numberOfPeople = totalPeople,
                    color
                };
            }));

            return Json(events.Where(e => e.color != null));
        }*/

        /*[HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest request)
        {
            if (ModelState.IsValid)
            {
                foreach (var roomData in request.RoomsData)
                {
                    var overlappingBookings = await _context.BookingRooms
                        .Include(br => br.Booking)
                        .Where(br => br.IdRoom == roomData.RoomId)
                        .Where(br =>
                            (request.CheckInDate < br.Booking.CheckOutDate && request.CheckOutDate > br.Booking.CheckInDate))
                        .ToListAsync();

                    if (overlappingBookings.Any())
                    {
                        return BadRequest("Wybrane daty są zajęte dla jednego z pokoi.");
                    }
                }

                var booking = new Booking
                {
                    IdGuest = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    NumberOfPeople = request.RoomsData.Sum(rd => rd.NumberOfPeople),
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    Bill = CalculateTotalBill(request.RoomsData, request.CheckInDate, request.CheckOutDate),
                    Paid = false,
                    Verified = false,
                    BookingRooms = request.RoomsData.Select(rd => new BookingRoom
                    {
                        IdRoom = rd.RoomId,
                        NumberOfPeople = rd.NumberOfPeople
                    }).ToList()
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest("Nieprawidłowe dane rezerwacji.");
        }*/

        /*private double CalculateTotalBill(List<RoomBookingInfo> roomsData, DateTime checkInDate, DateTime checkOutDate)
        {
            double totalBill = 0;

            foreach (var roomData in roomsData)
            {
                var room = _context.Rooms.Find(roomData.RoomId);
                if (room != null)
                {
                    var nights = (checkOutDate - checkInDate).Days;
                    totalBill += nights * room.PricePerNight * roomData.NumberOfPeople;
                }
            }

            return totalBill;
        }*/
    }

    // Definicje klas pomocniczych do danych przyjmowanych przez ConfirmBooking
   /* public class ConfirmBookingRequest
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<RoomBookingInfo> RoomsData { get; set; }
    }

    public class RoomBookingInfo
    {
        public int RoomId { get; set; }
        public int NumberOfPeople { get; set; }
    }

    public class DateTimeRangeModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }*/

}
