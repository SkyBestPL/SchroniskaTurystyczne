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
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdGuest")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.HasKey("IdBooking", "IdRoom");

                    b.HasIndex("IdRoom");

                    b.ToTable("BookingRooms");
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

                    b.Property<int>("IdReceiver")
                        .HasColumnType("int");

                    b.Property<int>("IdSender")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdReceiver");

                    b.HasIndex("IdSender");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdBooking")
                        .HasColumnType("int");

                    b.Property<int>("Installment")
                        .HasColumnType("int");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdBooking");

                    b.ToTable("Payments");
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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Point", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("IdShelter")
                        .HasColumnType("int");

                    b.Property<double>("LocationLat")
                        .HasColumnType("float");

                    b.Property<double>("LocationLon")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.ToTable("Points");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.HasIndex("IdUser");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<int>("IdType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdShelter");

                    b.HasIndex("IdType");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PricePerNight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("RoomTypes");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoutePoint", b =>
                {
                    b.Property<int>("IdRoute")
                        .HasColumnType("int");

                    b.Property<int>("IdPoint")
                        .HasColumnType("int");

                    b.Property<int>("PointId")
                        .HasColumnType("int");

                    b.Property<int>("SavedRouteId")
                        .HasColumnType("int");

                    b.HasKey("IdRoute", "IdPoint");

                    b.HasIndex("PointId");

                    b.HasIndex("SavedRouteId");

                    b.ToTable("RoutePoints");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GuestId")
                        .HasColumnType("int");

                    b.Property<int>("IdGuest")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GuestId");

                    b.ToTable("SavedRoutes");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdExhibitor")
                        .HasColumnType("int");

                    b.Property<string>("LocationLat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocationLon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("IdExhibitor");

                    b.ToTable("Shelters");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.ShelterTag", b =>
                {
                    b.Property<int>("IdShelter")
                        .HasColumnType("int");

                    b.Property<int>("IdTag")
                        .HasColumnType("int");

                    b.HasKey("IdShelter", "IdTag");

                    b.HasIndex("IdTag");

                    b.ToTable("ShelterTags");
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

            modelBuilder.Entity("SchroniskaTurystyczne.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdRole")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdRole");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.User", "Guest")
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
                    b.HasOne("SchroniskaTurystyczne.Models.User", "Receiver")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("IdReceiver")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("IdSender")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Payment", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Booking", "Booking")
                        .WithMany("Payments")
                        .HasForeignKey("IdBooking")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Photo", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Photos")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.User", null)
                        .WithMany("Photos")
                        .HasForeignKey("UserId");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Point", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Points")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Review", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("Reviews")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.User", "User")
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

                    b.HasOne("SchroniskaTurystyczne.Models.User", null)
                        .WithMany("Rooms")
                        .HasForeignKey("UserId");

                    b.Navigation("RoomType");

                    b.Navigation("Shelter");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoutePoint", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Point", "Point")
                        .WithMany("RoutePoints")
                        .HasForeignKey("PointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.SavedRoute", "SavedRoute")
                        .WithMany("RoutePoints")
                        .HasForeignKey("SavedRouteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Point");

                    b.Navigation("SavedRoute");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.User", "Guest")
                        .WithMany()
                        .HasForeignKey("GuestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guest");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.User", "Exhibitor")
                        .WithMany("Shelters")
                        .HasForeignKey("IdExhibitor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exhibitor");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.ShelterTag", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Shelter", "Shelter")
                        .WithMany("ShelterTags")
                        .HasForeignKey("IdShelter")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchroniskaTurystyczne.Models.Tag", "Tag")
                        .WithMany("ShelterTags")
                        .HasForeignKey("IdTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shelter");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.User", b =>
                {
                    b.HasOne("SchroniskaTurystyczne.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("IdRole")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Booking", b =>
                {
                    b.Navigation("BookingRooms");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Point", b =>
                {
                    b.Navigation("RoutePoints");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Room", b =>
                {
                    b.Navigation("BookingRooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.RoomType", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.SavedRoute", b =>
                {
                    b.Navigation("RoutePoints");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Shelter", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Points");

                    b.Navigation("Reviews");

                    b.Navigation("Rooms");

                    b.Navigation("ShelterTags");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.Tag", b =>
                {
                    b.Navigation("ShelterTags");
                });

            modelBuilder.Entity("SchroniskaTurystyczne.Models.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Photos");

                    b.Navigation("ReceivedMessages");

                    b.Navigation("Reviews");

                    b.Navigation("Rooms");

                    b.Navigation("SentMessages");

                    b.Navigation("Shelters");
                });
#pragma warning restore 612, 618
        }
    }
}
