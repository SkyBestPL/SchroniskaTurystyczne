using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.ViewModels
{
    public class ReviewsViewModel
    {
        public int ShelterId { get; set; }
        public string ShelterName { get; set; }
        public List<Review> Reviews { get; set; }
        public bool CanAddReview { get; set; }
        public Review? ExistingUserReview { get; set; }
    }
}
