namespace SchroniskaTurystyczne.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<ReviewInfoViewModel> Reviews { get; set; }
        public List<BookingViewModel> CurrentBookings { get; set; } // Only populated for shelter owners
        public List<ShelterVisitedViewModel> VisitedShelters { get; set; }
    }

    public class ReviewInfoViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int Rating { get; set; }
        public string Contents { get; set; }
        public ShelterBasicInfoViewModel Shelter { get; set; }
    }

    public class ShelterBasicInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; } // City, Country
    }

    public class BookingViewModel
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfPeople { get; set; }
        public bool Paid { get; set; }
        public bool Verified { get; set; }
    }

    public class ShelterVisitedViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime VisitDate { get; set; } // Based on past bookings
    }
}
