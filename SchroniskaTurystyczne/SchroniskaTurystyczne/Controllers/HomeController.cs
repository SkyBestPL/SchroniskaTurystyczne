using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;
using System.Diagnostics;

namespace SchroniskaTurystyczne.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            var model = new ShelterSearchViewModel
            {
                AvailableCategories = categories
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Search(ShelterSearchViewModel model)
        {
            var query = _context.Shelters.AsQueryable();

            if (model.SelectedCategoryId.HasValue)
            {
                query = query.Where(s => s.IdCategory == model.SelectedCategoryId.Value);
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                query = query.Where(s => s.Name.Contains(model.SearchTerm) || s.Description.Contains(model.SearchTerm));
            }

            var shelters = await query
                .Include(s => s.Category)
                .Include(s => s.Tags)
                .Select(s => new ShelterViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Rating = s.Rating,
                    City = s.City,
                    Country = s.Country,
                    Confirmed = s.ConfirmedShelter,
                    Tags = s.Tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name }).ToList(),
                    MainPhotoBase64 = s.Photos.FirstOrDefault() != null ? Convert.ToBase64String(s.Photos.FirstOrDefault().PhotoData) : null
                })
                .ToListAsync();

            model.Shelters = shelters;

            model.AvailableCategories = await _context.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            Debug.WriteLine("koniec");
            return RedirectToAction("Index", "Shelters", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
