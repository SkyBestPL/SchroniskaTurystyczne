﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchroniskaTurystyczne.Data;

#nullable disable

namespace SchroniskaTurystyczne.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("RoomFacility", b =>
                {
                    b.Property<int>("IdRoom")
                        .HasColumnType("int");

                    b.Property<int>("IdFacility")
                        .HasColumnType("int");

                    b.HasKey("IdRoom", "IdFacility");

                    b.HasIndex("IdFacility");

                    b.ToTable("RoomFacility");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "18b3e893-3b24-47cf-8ac2-3318d36a90c6",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "b250291e-9553-40fc-8c59-5afb56b1bd89",
                            Email = "admin@admin.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@ADMIN.COM",
                            NormalizedUserName = "ADMIN@ADMIN.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAEOQVh1kJofgMzQRuloKQXrqZHtVl0xI+t3ITZ/tda/c6d2o1b6xCQGWzLPNWkVqsIw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "692ba88b-62e5-4816-9a64-6c6dc39363d4",
                            TwoFactorEnabled = false,
                            UserName = "admin@admin.com"
                        });
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Bill")
                        .HasColumnType("float");

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdGuest")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.Property<bool>("Paid")
                        .HasColumnType("bit");

                    b.Property<int?>("PaymentDate")
                        .HasColumnType("int");

                    b.Property<bool>("Verified")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("IdGuest");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.BookingRoom", b =>
                {
                    b.Property<int>("IdBooking")
                        .HasColumnType("int");

                    b.Property<int>("IdRoom")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.HasKey("IdBooking");

                    b.HasIndex("IdRoom");

                    b.ToTable("BookingRooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Facility", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Facilities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Toaleta",
                            Name = "Toilet"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Prysznic",
                            Name = "Shower"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Pościel",
                            Name = "Sheets"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Wi-Fi",
                            Name = "Wi-Fi"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Parking",
                            Name = "Parking"
                        });
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdReceiver")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdSender")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("IdReceiver");

                    b.HasIndex("IdSender");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Point", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdSavedRoute")
                        .HasColumnType("int");

                    b.Property<int?>("IdShelter")
                        .HasColumnType("int");

                    b.Property<double>("LocationLat")
                        .HasColumnType("float");

                    b.Property<double>("LocationLon")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<int?>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdSavedRoute");

                    b.HasIndex("IdShelter");

                    b.HasIndex("RoomId");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contents")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<string>("IdUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.HasIndex("IdUser");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "ba925120-9ecd-409d-80af-2834d054ee05",
                            Name = "Admin",
                            NormalizedName = "Admin"
                        },
                        new
                        {
                            Id = "1837c4ee-a9fc-4722-be8d-23a167605b3a",
                            Name = "User",
                            NormalizedName = "User"
                        });
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<int>("IdType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("PricePerNight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.HasIndex("IdType");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomPhoto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdRoom")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdRoom");

                    b.ToTable("RoomPhotos");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoomTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Pokój publiczny",
                            Name = "Public"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Pokój prywatny",
                            Name = "Private"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Miejsca na działce",
                            Name = "Plot"
                        });
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("IdGuest")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdGuest");

                    b.ToTable("SavedRoutes");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdExhibitor")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LocationLat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocationLon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdExhibitor");

                    b.ToTable("Shelters");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "18b3e893-3b24-47cf-8ac2-3318d36a90c6",
                            RoleId = "ba925120-9ecd-409d-80af-2834d054ee05"
                        });
                });

            modelBuilder.Entity("ShelterTag", b =>
                {
                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<int>("IdTag")
                        .HasColumnType("int");

                    b.HasKey("IdShelter", "IdTag");

                    b.HasIndex("IdTag");

                    b.ToTable("ShelterTag");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RoomFacility", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Facility", null)
                        .WithMany()
                        .HasForeignKey("IdFacility")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.Room", null)
                        .WithMany()
                        .HasForeignKey("IdRoom")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "Guest")
                        .WithMany("Bookings")
                        .HasForeignKey("IdGuest")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.BookingRoom", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Booking", "Booking")
                        .WithMany("BookingRooms")
                        .HasForeignKey("IdBooking")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.Room", "Room")
                        .WithMany("BookingRooms")
                        .HasForeignKey("IdRoom")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Message", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "Receiver")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("IdReceiver")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("IdSender")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Photo", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Photos")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Point", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.SavedRoute", "SavedRoute")
                        .WithMany("Points")
                        .HasForeignKey("IdSavedRoute")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Points")
                        .HasForeignKey("IdShelter");

                    b.HasOne("SchroniskaTurystyczne.Models.Room", null)
                        .WithMany("Points")
                        .HasForeignKey("RoomId");

                    b.Navigation("SavedRoute");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Review", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Reviews")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Shelter");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Room", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Rooms")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.RoomType", "RoomType")
                        .WithMany("Rooms")
                        .HasForeignKey("IdType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoomType");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomPhoto", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Room", "Room")
                        .WithMany("RoomPhotos")
                        .HasForeignKey("IdRoom")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "Guest")
                        .WithMany("SavedRoutes")
                        .HasForeignKey("IdGuest")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "Exhibitor")
                        .WithMany("Shelters")
                        .HasForeignKey("IdExhibitor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exhibitor");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.UserRole", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.AppUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ShelterTag", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", null)
                        .WithMany()
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("IdTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.AppUser", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("ReceivedMessages");

                    b.Navigation("Reviews");

                    b.Navigation("SavedRoutes");

                    b.Navigation("SentMessages");

                    b.Navigation("Shelters");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.Navigation("BookingRooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Room", b =>
                {
                    b.Navigation("BookingRooms");

                    b.Navigation("Points");

                    b.Navigation("RoomPhotos");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomType", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.Navigation("Points");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Points");

                    b.Navigation("Reviews");

                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
