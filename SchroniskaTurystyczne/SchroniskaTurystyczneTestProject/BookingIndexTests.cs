using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using SchroniskaTurystyczne.Controllers;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchroniskaTurystyczneTestProject
{
    public class BookingIndexTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbContextTransaction _transaction;

        public BookingIndexTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-SchroniskaTurystyczne-cc09fa87-c400-4b86-b2e5-a30ad7212d08;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            _context = new ApplicationDbContext(options);

            _transaction = _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _context.Dispose();
        }

        [Fact]
        public async Task TestBookingSuccess()
        {
            // Arrange
            var mockUserStore = new Mock<IUserStore<AppUser>>();
            var mockUserManager = new Mock<UserManager<AppUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            var testUser = new AppUser
            {
                Id = "TestUser",
                UserName = "TestUser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                EmailConfirmed = true,
                NormalizedUserName = "TESTUSER",
                NormalizedEmail = "TEST@EXAMPLE.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var controller = new BookingController(_context, mockUserManager.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, testUser.Id) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var request = new ConfirmBookingRequest
            {
                CheckInDate = DateTime.UtcNow.AddDays(1),
                CheckOutDate = DateTime.UtcNow.AddDays(3),
                Rooms = new List<ConfirmBookingRequest.RoomBooking>
                {
                    new ConfirmBookingRequest.RoomBooking
                    {
                        RoomId = 2,
                        NumberOfPeople = 2
                    }
                }
            };

            // Act
            var result = await controller.ConfirmBooking(request) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.True((bool)response.success);

            var booking = await _context.Bookings
                .Include(b => b.BookingRooms)
                .OrderByDescending(b => b.Id)
                .FirstOrDefaultAsync();

            Assert.NotNull(booking);
            Assert.Equal(testUser.Id, booking.IdGuest);
            Assert.Equal(request.CheckInDate.Date, booking.CheckInDate.Date);
            Assert.Equal(request.CheckOutDate.Date, booking.CheckOutDate.Date);
            Assert.Single(booking.BookingRooms);
            Assert.Equal(2, booking.BookingRooms.First().IdRoom);
            Assert.Equal(2, booking.BookingRooms.First().NumberOfPeople);
        }

        [Fact]
        public async Task TestBookingFail()
        {
            // Arrange
            var mockUserStore = new Mock<IUserStore<AppUser>>();
            var mockUserManager = new Mock<UserManager<AppUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            var testUser = new AppUser
            {
                Id = "TestUser",
                UserName = "TestUser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                EmailConfirmed = true,
                NormalizedUserName = "TESTUSER",
                NormalizedEmail = "TEST@EXAMPLE.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            _context.Users.Add(testUser);
            await _context.SaveChangesAsync();

            var controller = new BookingController(_context, mockUserManager.Object);

            var existingBooking = new Booking
            {
                IdGuest = testUser.Id,
                CheckInDate = DateTime.UtcNow.AddDays(1),
                CheckOutDate = DateTime.UtcNow.AddDays(3),
                BookingDate = DateTime.UtcNow,
                NumberOfPeople = 2,
                Valid = true,
                Ended = false,
                IdShelter = 1
            };
            _context.Bookings.Add(existingBooking);
            await _context.SaveChangesAsync();

            var existingBookingRoom = new BookingRoom
            {
                IdBooking = existingBooking.Id,
                IdRoom = 2,
                NumberOfPeople = 2
            };
            _context.BookingRooms.Add(existingBookingRoom);
            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, testUser.Id) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var request = new ConfirmBookingRequest
            {
                CheckInDate = DateTime.UtcNow.AddDays(1),
                CheckOutDate = DateTime.UtcNow.AddDays(3),
                Rooms = new List<ConfirmBookingRequest.RoomBooking>
                {
                    new ConfirmBookingRequest.RoomBooking
                    {
                        RoomId = 2,
                        NumberOfPeople = 2
                    }
                }
            };

            // Act
            var result = await controller.ConfirmBooking(request) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)response.success);
            Assert.Equal("Przekroczona pojemność pokoju w wybranym okresie.", (string)response.message);
        }
    }
}
