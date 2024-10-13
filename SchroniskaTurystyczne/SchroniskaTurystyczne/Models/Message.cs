using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string IdSender { get; set; }
        public string IdReceiver { get; set; }
        public string Date { get; set; }
        public string Contents { get; set; }

        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
    }
}
