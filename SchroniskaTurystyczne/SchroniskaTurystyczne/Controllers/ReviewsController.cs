using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;
using SchroniskaTurystyczne.ViewModels;

namespace SchroniskaTurystyczne.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ReviewsController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult ShelterReviews(int shelterId)
        {
            var shelter = _context.Shelters
                .Include(s => s.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefault(s => s.Id == shelterId);

            if (shelter == null)
            {
                return NotFound("Schronisko nie zostało znalezione.");
            }

            var user = _userManager.GetUserAsync(User).Result;
            bool hasBooking = false;
            Review? existingReview = null;

            if (user != null)
            {
                hasBooking = _context.Bookings.Any(b =>
                    b.IdShelter == shelterId &&
                    b.IdGuest == user.Id &&
                    b.Ended == true);

                existingReview = shelter.Reviews?.FirstOrDefault(r => r.IdUser == user.Id);
            }

            var viewModel = new ReviewsViewModel
            {
                ShelterId = shelterId,
                ShelterName = shelter.Name,
                Reviews = shelter.Reviews?.ToList() ?? new List<Review>(),
                CanAddReview = user != null && hasBooking && existingReview == null,
                ExistingUserReview = existingReview
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Wystąpił błąd przy zapisie recenzji.";
                return RedirectToAction("ShelterReviews", new { shelterId = model.ShelterId });
            }

            if (model.Contents != null)
            {
                if (model.Contents.Length > 1000)
                {
                    TempData["ErrorMessage"] = "Recenzja jest za długa.";
                    return RedirectToAction("ShelterReviews", new { shelterId = model.ShelterId });
                }
            }

            var user = _userManager.GetUserAsync(User).Result;

            var hasBooking = _context.Bookings.Any(b =>
                b.IdShelter == model.ShelterId &&
                b.IdGuest == user.Id);

            var existingReview = _context.Reviews.FirstOrDefault(r =>
                r.IdShelter == model.ShelterId &&
                r.IdUser == user.Id);

            if (!hasBooking || existingReview != null)
            {
                return Forbid();
            }

            var review = new Review
            {
                IdShelter = model.ShelterId,
                IdUser = user.Id,
                Rating = model.Rating,
                Contents = model.Contents,
                Date = DateTime.Now.ToString("dd.MM.yyyy")
            };

            _context.Reviews.Add(review);

            var shelter = _context.Shelters.Find(model.ShelterId);
            shelter.AmountOfReviews++;
            shelter.Rating = CalculateNewShelterRating(shelter, model.Rating);

            _context.SaveChanges();

            return RedirectToAction("ShelterReviews", new { shelterId = model.ShelterId });
        }

        private double CalculateNewShelterRating(Shelter shelter, int newRating)
        {
            var currentTotalRating = (shelter.Rating ?? 0) * (shelter.AmountOfReviews - 1);
            return (currentTotalRating + newRating) / shelter.AmountOfReviews;
        }

        [HttpPost]
        public IActionResult EditReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Wystąpił błąd przy zapisie recenzji.";
                return RedirectToAction("ShelterReviews", new { shelterId = model.ShelterId });
            }

            if(model.Contents != null)
            {
                if (model.Contents.Length > 1000)
                {
                    TempData["ErrorMessage"] = "Recenzja jest za długa.";
                    return RedirectToAction("ShelterReviews", new { shelterId = model.ShelterId });
                }
            }

            var user = _userManager.GetUserAsync(User).Result;

            var review = _context.Reviews.FirstOrDefault(r =>
                r.Id == model.Id && r.IdUser == user.Id);

            if (review == null)
            {
                return Forbid();
            }

            review.Rating = model.Rating;
            review.Contents = model.Contents;
            review.Date = DateTime.Now.ToString("dd.MM.yyyy");

            var shelter = _context.Shelters.Find(review.IdShelter);
            shelter.Rating = RecalculateShelterRating(shelter);

            _context.SaveChanges();

            return RedirectToAction("ShelterReviews", new { shelterId = review.IdShelter });
        }

        private double RecalculateShelterRating(Shelter shelter)
        {
            var reviews = _context.Reviews.Where(r => r.IdShelter == shelter.Id).ToList();
            if (reviews.Count == 0) return 0;

            var totalRating = reviews.Sum(r => r.Rating);
            return (double)totalRating / reviews.Count;
        }

        [HttpGet]
        public IActionResult DeleteReview(int reviewId, int shelterId)
        {
            var user = _userManager.GetUserAsync(User).Result;

            var review = _context.Reviews.FirstOrDefault(r =>
                r.Id == reviewId && r.IdUser == user.Id);

            if (review == null)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);

            var shelter = _context.Shelters.Find(shelterId);
            if (shelter != null)
            {
                shelter.AmountOfReviews--;
                shelter.Rating = RecalculateShelterRating(shelter);
            }

            _context.SaveChanges();

            return RedirectToAction("ShelterReviews", new { shelterId = shelterId });
        }
    }
}
