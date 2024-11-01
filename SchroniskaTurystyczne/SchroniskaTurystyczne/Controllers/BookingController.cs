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

        [HttpGet]
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
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest request)
        {
            if (ModelState.IsValid)
            {
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

                return Ok(); // Sukces
            }

            return BadRequest("Nieprawidłowe dane rezerwacji.");
        }

        private double CalculateTotalBill(List<RoomBookingInfo> roomsData, DateTime checkInDate, DateTime checkOutDate)
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
        }
    }

    // Definicje klas pomocniczych do danych przyjmowanych przez ConfirmBooking
    public class ConfirmBookingRequest
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

}
