using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SchroniskaTurystyczne.Models;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace SchroniskaTurystyczne.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Message> Messages { get; set; }
        //public DbSet<Payment> Payments { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Room> Rooms { get; set; }
        //public DbSet<RoomFacility> RoomFacilities { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        //public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<SavedRoute> SavedRoutes { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        //public DbSet<ShelterTag> ShelterTags { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //wiele do wielu
            modelBuilder.Entity<Shelter>()
                .HasMany(e => e.Tags)
                .WithMany(e => e.Shelters)
                .UsingEntity(
                    "ShelterTag",
                    l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("IdTag").HasPrincipalKey(nameof(Tag.Id)),
                    r => r.HasOne(typeof(Shelter)).WithMany().HasForeignKey("IdShelter").HasPrincipalKey(nameof(Shelter.Id)),
                    j => j.HasKey("IdShelter", "IdTag"));

            modelBuilder.Entity<Room>()
                .HasMany(e => e.Facilities)
                .WithMany(e => e.Rooms)
                .UsingEntity(
                    "RoomFacility",
                    l => l.HasOne(typeof(Facility)).WithMany().HasForeignKey("IdFacility").HasPrincipalKey(nameof(Facility.Id)),
                    r => r.HasOne(typeof(Room)).WithMany().HasForeignKey("IdRoom").HasPrincipalKey(nameof(Room.Id)),
                    j => j.HasKey("IdRoom", "IdFacility"));

            /*modelBuilder.Entity<Booking>()
                .HasMany(e => e.BookingRooms)
                .WithOne(e => e.Booking)
                .HasForeignKey(e => e.IdBooking);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.BookingRooms)
                .WithOne(e => e.Room)
                .HasForeignKey(e => e.IdRoom);*/

            // Konfiguracja klucza złożonego dla BookingRoom
            modelBuilder.Entity<BookingRoom>()
                .HasKey(br => new { br.IdBooking, br.IdRoom });

            // Opcjonalnie: konfiguracja relacji, jeśli potrzebne
            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Booking)
                .WithMany(b => b.BookingRooms)
                .HasForeignKey(br => br.IdBooking);

            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRooms)
                .HasForeignKey(br => br.IdRoom);

            //klucze zlozone
            /*modelBuilder.Entity<BookingRoom>()
                .HasKey(br => new { br.IdBooking, br.IdRoom });*/

            //rel jeden do wielu
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.IdGuest)
                .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.IdBooking)
                .OnDelete(DeleteBehavior.Cascade);*/

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

            modelBuilder.Entity<RoomPhoto>()
                .HasOne(p => p.Room)
                .WithMany(s => s.RoomPhotos)
                .HasForeignKey(p => p.IdRoom)
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
                .IsRequired(false);

            modelBuilder.Entity<SavedRoute>()
                .HasOne(sr => sr.Guest)
                .WithMany(u => u.SavedRoutes)
                .HasForeignKey(sr => sr.IdGuest);

            //jeden-do-wielu
            modelBuilder.Entity<Point>()
                .HasOne(p => p.SavedRoute)
                .WithMany(r => r.Points)
                .HasForeignKey(p => p.IdSavedRoute);

            //pola tekstowe
            /*modelBuilder.Entity<Room>()
                .Property(r => r.Status)
            .HasMaxLength(50);*/

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

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.IdUser);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.Guest)
                .HasForeignKey(b => b.IdGuest);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.IdReceiver)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Shelters)
                .WithOne(s => s.Exhibitor)
                .HasForeignKey(s => s.IdExhibitor);
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.SavedRoutes)
                .WithOne(sr => sr.Guest)
                .HasForeignKey(sr => sr.IdGuest);

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder builder)
        {
            var adminUser = new AppUser()
            {
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEOQVh1kJofgMzQRuloKQXrqZHtVl0xI+t3ITZ/tda/c6d2o1b6xCQGWzLPNWkVqsIw==",
                EmailConfirmed = true
            };

            var rolAdmin = new Role()
            {
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var userRole = new Role()
            {
                Name = "User",
                NormalizedName = "User"
            };

            builder.Entity<Role>().HasData(rolAdmin, userRole);
            builder.Entity<AppUser>().HasData(adminUser);

            var userRoleAdmin = new UserRole()
            {
                RoleId = rolAdmin.Id,
                UserId = adminUser.Id
            };

            builder.Entity<UserRole>().HasData(userRoleAdmin);

            var roomPublic = new RoomType()
            {
                Id = 1,
                Name = "Pokój publiczny"
            };

            var roomPrivate = new RoomType()
            {
                Id = 2,
                Name = "Pokój prywatny"
            };

            var roomPlot = new RoomType()
            {
                Id = 3,
                Name = "Miejsce na działce"
            };

            builder.Entity<RoomType>().HasData(roomPublic);
            builder.Entity<RoomType>().HasData(roomPrivate);
            builder.Entity<RoomType>().HasData(roomPlot);

            var facToilet = new Facility()
            {
                Id = 1,
                Name = "Toaleta"
            };

            var facShower = new Facility()
            {
                Id = 2,
                Name = "Prysznic"
            };

            var facSheets = new Facility()
            {
                Id = 3,
                Name = "Pościel"
            };

            builder.Entity<Facility>().HasData(facToilet);
            builder.Entity<Facility>().HasData(facShower);
            builder.Entity<Facility>().HasData(facSheets);

            var wifiTag = new Tag()
            {
                Id = 1,
                Name = "Wi-Fi"
            };

            var plotTag = new Tag()
            {
                Id = 2,
                Name = "Pola namiotowe"
            };

            var roomTag = new Tag()
            {
                Id = 3,
                Name = "Pokoje"
            };

            var barTag = new Tag()
            {
                Id = 4,
                Name = "Bufet"
            };

            var parkingTag = new Tag()
            {
                Id = 5,
                Name = "Parking"
            };

            builder.Entity<Tag>().HasData(wifiTag);
            builder.Entity<Tag>().HasData(plotTag);
            builder.Entity<Tag>().HasData(roomTag);
            builder.Entity<Tag>().HasData(barTag);
            builder.Entity<Tag>().HasData(parkingTag);
        }
    }
}
