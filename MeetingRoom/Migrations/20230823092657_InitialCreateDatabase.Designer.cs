﻿// <auto-generated />
using System;
using MeetingRoomWebApp.AutoGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MeetingRoom.Migrations
{
    [DbContext(typeof(MeetingRoomDbContext))]
    [Migration("20230823092657_InitialCreateDatabase")]
    partial class InitialCreateDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.BookedRoom", b =>
                {
                    b.Property<long>("BookedRoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("RoomId")
                        .HasColumnType("INTEGER");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BookedRoomId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("BookedRooms");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.Role", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.Room", b =>
                {
                    b.Property<long>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("Capacity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoomName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoomId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<long>("RoleId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.BookedRoom", b =>
                {
                    b.HasOne("MeetingRoomWebApp.AutoGen.Room", "Rooms")
                        .WithMany("BookedRooms")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("MeetingRoomWebApp.AutoGen.User", "Users")
                        .WithMany("BookedRooms")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Rooms");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.User", b =>
                {
                    b.HasOne("MeetingRoomWebApp.AutoGen.Role", "Roles")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.Room", b =>
                {
                    b.Navigation("BookedRooms");
                });

            modelBuilder.Entity("MeetingRoomWebApp.AutoGen.User", b =>
                {
                    b.Navigation("BookedRooms");
                });
#pragma warning restore 612, 618
        }
    }
}