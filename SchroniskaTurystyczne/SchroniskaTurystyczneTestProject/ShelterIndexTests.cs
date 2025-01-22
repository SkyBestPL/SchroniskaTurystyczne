using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using SchroniskaTurystyczne;
using SchroniskaTurystyczne.Controllers;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;
using System;
using System.Security.Claims;
namespace SchroniskaTurystyczneTestProject
{
    public class ShelterIndexTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ShelterIndexTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
        }

        [Fact]
        public async void TestShelterIndexLoad()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act https://localhost:7119/Shelters/Index
            var response = await client.GetAsync("/Shelters/Index");
            int code = (int) response.StatusCode;
            // Assert
            Assert.Equal(200, code);
        }

        [Fact]
        public async Task TestSearchAndFilter()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-SchroniskaTurystyczne-cc09fa87-c400-4b86-b2e5-a30ad7212d08;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var mockUserStore = new Mock<IUserStore<AppUser>>();
            var mockUserManager = new Mock<UserManager<AppUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new AppUser { Id = "TestUser", UserName = "TestUser" });

            using var context = new ApplicationDbContext(options);

            var controller = new SheltersController(context, mockUserManager.Object);

            var searchModel = new ShelterSearchViewModel
            {
                SearchTerm = "Zakopane",
                SelectedTagIds = new List<int> { 1 },
                SelectedRoomTypeIds = new List<int> { 2 }
            };

            // Act
            var result = controller.Index(searchModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var viewModel = Assert.IsType<ShelterSearchViewModel>(result.Model);
            Assert.NotEmpty(viewModel.Shelters);
            Assert.All(viewModel.Shelters, shelter =>
            {
                Assert.Contains("Zakopane", shelter.City);

                Assert.NotEmpty(shelter.Tags);
                Assert.True(shelter.Tags.Any(tag => searchModel.SelectedTagIds.Contains(tag.Id)),
                    $"Schronisko '{shelter.Name}' nie ma ¿adnego z wybranych tagów.");
            });
        }
    }
}