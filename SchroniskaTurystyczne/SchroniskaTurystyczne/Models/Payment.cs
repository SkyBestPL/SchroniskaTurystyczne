using System.ComponentModel.DataAnnotations;

namespace SchroniskaTurystyczne.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int IdBooking { get; set; }
        public double Amount { get; set; }
        public int Installment { get; set; }
        public string Method { get; set; }
        public string Date { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
