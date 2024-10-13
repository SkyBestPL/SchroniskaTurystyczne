using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Models;
using System.Data;
using System.Reflection.Emit;

namespace SchroniskaTurystyczne.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<SavedRoute> SavedRoutes { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<ShelterTag> ShelterTags { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //klucze zlozone
            modelBuilder.Entity<BookingRoom>()
                .HasKey(br => new { br.IdBooking, br.IdRoom });

            modelBuilder.Entity<RoutePoint>()
                .HasKey(rp => new { rp.IdRoute, rp.IdPoint });

            //rel wiele do wielu
            modelBuilder.Entity<ShelterTag>()
                .HasKey(st => new { st.IdShelter, st.IdTag });

            modelBuilder.Entity<ShelterTag>()
                .HasOne(st => st.Shelter)
                .WithMany(s => s.ShelterTags)
                .HasForeignKey(st => st.IdShelter);

            modelBuilder.Entity<ShelterTag>()
                .HasOne(st => st.Tag)
                .WithMany(t => t.ShelterTags)
                .HasForeignKey(st => st.IdTag);

            //rel jeden do wielu
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.IdGuest)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.IdBooking)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Shelter)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.IdShelter)
                .OnDelete(DeleteBehavior.Cascade);

            //rel dla IdUser
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Shelter)
                .WithMany(s => s.Rooms)
                .HasForeignKey(r => r.IdShelter)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.IdType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Shelter)
                .WithMany(s => s.Photos)
                .HasForeignKey(p => p.IdShelter)
                .OnDelete(DeleteBehavior.Cascade);

            //inne
           /* modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.IdRole)
                .OnDelete(DeleteBehavior.Restrict);*/

            modelBuilder.Entity<Point>()
                .HasOne(p => p.Shelter)
                .WithMany(s => s.Points)
                .HasForeignKey(p => p.IdShelter)
                .OnDelete(DeleteBehavior.Restrict); //moze byc null dlatego restricted

            //pola tekstowe
            modelBuilder.Entity<Room>()
                .Property(r => r.Status)
            .HasMaxLength(50);

            modelBuilder.Entity<Review>()
                .Property(r => r.Contents)
                .HasMaxLength(500);

            //indeksy
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.Name)
                .IsUnique();

            //rel dla SenderId
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Restrict);

            //rel dla ReceiverId
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.IdReceiver)
                .OnDelete(DeleteBehavior.Cascade);

            //rel z Bookings
            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Booking)
                .WithMany(b => b.BookingRooms)
                .HasForeignKey(br => br.IdBooking)
                .OnDelete(DeleteBehavior.Cascade);

            //rel z Rooms
            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRooms)
                .HasForeignKey(br => br.IdRoom)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shelter>()
                .HasOne(s => s.Exhibitor)
                .WithMany(u => u.Shelters)
                .HasForeignKey(s => s.IdExhibitor)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
