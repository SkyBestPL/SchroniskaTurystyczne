using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string IdSender { get; set; }
        [Required]
        public string IdReceiver { get; set; }
        public DateTime Date { get; set; }
        public string Contents { get; set; }

        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
    }
}
