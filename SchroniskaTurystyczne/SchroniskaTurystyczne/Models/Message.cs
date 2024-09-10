using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int IdSender { get; set; }
        public int IdReceiver { get; set; }
        public string Date { get; set; }
        public string Contents { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}
