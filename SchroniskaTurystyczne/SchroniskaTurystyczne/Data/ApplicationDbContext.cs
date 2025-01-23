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
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<SavedRoute> SavedRoutes { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //jeden do jednego
            modelBuilder.Entity<AppUser>()
                .HasOne(s => s.Shelter)
                .WithOne(u => u.Exhibitor)
                .HasForeignKey<Shelter>(s => s.IdExhibitor)
                .OnDelete(DeleteBehavior.Cascade); //użytkownik może mieć tylko 1 schronisko

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

            modelBuilder.Entity<BookingRoom>()
                .HasKey(br => new { br.IdBooking, br.IdRoom });

            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Booking)
                .WithMany(b => b.BookingRooms)
                .HasForeignKey(br => br.IdBooking)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRooms)
                .HasForeignKey(br => br.IdRoom)
                .OnDelete(DeleteBehavior.NoAction);

            //jeden do wielu
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.IdGuest)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Shelter)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.IdShelter)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Shelter)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.IdShelter)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.IdUser)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .Property(r => r.Contents)
                .HasMaxLength(1000);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Shelter)
                .WithMany(s => s.Rooms)
                .HasForeignKey(r => r.IdShelter)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.IdType)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Point>()
                .HasOne(p => p.Shelter)
                .WithMany(s => s.Points)
                .HasForeignKey(p => p.IdShelter)
                .IsRequired(false);

            modelBuilder.Entity<SavedRoute>()
                .HasOne(sr => sr.Guest)
                .WithMany(u => u.SavedRoutes)
                .HasForeignKey(sr => sr.IdGuest)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Point>()
                .HasOne(p => p.SavedRoute)
                .WithMany(r => r.Points)
                .HasForeignKey(p => p.IdSavedRoute)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.IdReceiver)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Shelter>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Shelters)
                .HasForeignKey(s => s.IdCategory)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shelter>()
                .HasOne(s => s.Exhibitor)
                .WithOne(u => u.Shelter)
                .HasForeignKey<Shelter>(s => s.IdExhibitor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

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
            var userAdmin = new AppUser() //konto admina
            {
                FirstName = "Schroniarz",
                LastName =  "Admin",
                Email = "schroniarz.admin@gmail.com",
                NormalizedEmail = "SCHRONIARZ.ADMIN@GMAIL.COM",
                UserName = "schroniarz.admin@gmail.com",
                NormalizedUserName = "SCHRONIARZ.ADMIN@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEOQVh1kJofgMzQRuloKQXrqZHtVl0xI+t3ITZ/tda/c6d2o1b6xCQGWzLPNWkVqsIw==", //haslo: Admin!1234
                EmailConfirmed = true
            };

            var adminRole = new Role()
            {
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var guestRole = new Role()
            {
                Name = "Guest",
                NormalizedName = "Guest"
            };

            var exhibitorRole = new Role()
            {
                Name = "Exhibitor",
                NormalizedName = "Exhibitor"
            };

            builder.Entity<Role>().HasData(adminRole, guestRole, exhibitorRole);
            builder.Entity<AppUser>().HasData(userAdmin);

            var userRoleAdmin = new UserRole()
            {
                RoleId = adminRole.Id,
                UserId = userAdmin.Id
            };

            builder.Entity<UserRole>().HasData(userRoleAdmin);

            var roomPublic = new RoomType()
            {
                Id = 1,
                Name = "Publiczny",
                Description = "Pokój rezerwowany wspólnie z innymi gośćmi"
            };

            var roomPrivate = new RoomType()
            {
                Id = 2,
                Name = "Prywatny",
                Description = "Pokój rezerwowany na własność"
            };

            var roomCamping = new RoomType()
            {
                Id = 3,
                Name = "Pole",
                Description = "Wspólne miejsce dla gości na zewnątrz"
            };

            builder.Entity<RoomType>().HasData(roomPublic);
            builder.Entity<RoomType>().HasData(roomPrivate);
            builder.Entity<RoomType>().HasData(roomCamping);

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

            var tatryCategory = new Category()
            {
                Id = 1,
                Name = "Tatry",
                Description = "Schroniska znajdujace się na paśmie górskim Tatr"
            };

            var bieszczadyCategory = new Category()
            {
                Id = 2,
                Name = "Bieszczady",
                Description = "Schroniska znajdujace się na paśmaie górskim Bieszczad"
            };

            var beskidZywieckiCategory = new Category()
            {
                Id = 3,
                Name = "Beskid Żywiecki",
                Description = "Schroniska znajdujace się na paśmaie górskim Beskidu Żywieckiego"
            };

            var beskidSlaskiCategory = new Category()
            {
                Id = 4,
                Name = "Beskid Śląski",
                Description = "Schroniska znajdujace się na paśmie górskim Beskidu Śląskiego"
            };

            var karkonoszeCategory = new Category()
            {
                Id = 5,
                Name = "Karkonosze",
                Description = "Schroniska znajdujace się na paśmie górskim Karkonoszy"
            };

            var otherCategory = new Category()
            {
                Id = 6,
                Name = "Inne",
                Description = "Schroniska znajdujace się na regionach innych niż pasma górskie"
            };

            builder.Entity<Category>().HasData(tatryCategory);
            builder.Entity<Category>().HasData(bieszczadyCategory);
            builder.Entity<Category>().HasData(beskidZywieckiCategory);
            builder.Entity<Category>().HasData(beskidSlaskiCategory);
            builder.Entity<Category>().HasData(karkonoszeCategory);
            builder.Entity<Category>().HasData(otherCategory);

            //------------------------------------------------------------------Dane testowego schroniska
            var exhibitorUser = new AppUser()
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                NormalizedEmail = "JAN.KOWALSKI@EXAMPLE.COM",
                UserName = "jan.kowalski@example.com",
                NormalizedUserName = "JAN.KOWALSKI@EXAMPLE.COM",
                PasswordHash = "1234",
                EmailConfirmed = true
            };
            builder.Entity<AppUser>().HasData(exhibitorUser);

            var exhibitorUserRole = new UserRole()
            {
                RoleId = exhibitorRole.Id,
                UserId = exhibitorUser.Id
            };
            builder.Entity<UserRole>().HasData(exhibitorUserRole);

            var shelter = new Shelter()
            {
                Id = 1,
                IdExhibitor = exhibitorUser.Id,
                IdCategory = 1,
                Name = "Schronisko Pod Tatrami",
                Description = "Malownicze schronisko położone w sercu Tatr.",
                ConfirmedShelter = true,
                Country = "Polska",
                City = "Zakopane",
                Street = "Tatrzańska",
                StreetNumber = "12",
                ZipCode = "34-500",
                LocationLon = "19.9383",
                LocationLat = "49.2965"
            };
            builder.Entity<Shelter>().HasData(shelter);

            var shelterTags = new List<object>
            {
                new { IdShelter = shelter.Id, IdTag = 1 },
                new { IdShelter = shelter.Id, IdTag = 3 },
                new { IdShelter = shelter.Id, IdTag = 5 }
            };
            builder.Entity("ShelterTag").HasData(shelterTags);

            var room1 = new Room()
            {
                Id = 1,
                IdShelter = shelter.Id,
                IdType = 1,
                Name = "Pokój wspólny",
                PricePerNight = 50,
                Capacity = 6,
                IsActive = true
            };

            var room2 = new Room()
            {
                Id = 2,
                IdShelter = shelter.Id,
                IdType = 2,
                Name = "Pokój prywatny",
                PricePerNight = 150,
                Capacity = 2,
                IsActive = true
            };

            var room3 = new Room()
            {
                Id = 3,
                IdShelter = shelter.Id,
                IdType = 3,
                Name = "Pole namiotowe",
                PricePerNight = 20,
                Capacity = 10,
                IsActive = true
            };
            builder.Entity<Room>().HasData(room1, room2, room3);

            var roomFacilities = new List<object>
            {
                new { IdRoom = room1.Id, IdFacility = 1 },
                new { IdRoom = room1.Id, IdFacility = 2 },
                new { IdRoom = room2.Id, IdFacility = 2 },
                new { IdRoom = room2.Id, IdFacility = 3 },
                new { IdRoom = room3.Id, IdFacility = 1 }
            };
            builder.Entity("RoomFacility").HasData(roomFacilities);
        }
    }
}
