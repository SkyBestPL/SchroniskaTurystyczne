using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int? ShelterId { get; set; }

        [Required(ErrorMessage = "Ocena jest wymagana")]
        [Range(1, 10, ErrorMessage = "Ocena musi być między 1 a 10")]
        public int Rating { get; set; }
        public string? Contents { get; set; }
    }
}
