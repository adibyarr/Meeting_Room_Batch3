using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomWebApp.AutoGen;

public partial class MeetingRoomDbContext : DbContext
{
    public MeetingRoomDbContext(DbContextOptions<MeetingRoomDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<BookedRoom> BookedRooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasOne(u => u.Roles).WithMany(r => r.Users).OnDelete(DeleteBehavior.ClientCascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
        });

        modelBuilder.Entity<BookedRoom>(entity =>
        {
            entity.HasKey(e => e.BookedRoomId);
            entity.HasOne(b => b.Users).WithMany(u => u.BookedRooms).OnDelete(DeleteBehavior.ClientCascade);
            entity.HasOne(b => b.Rooms).WithMany(r => r.BookedRooms).OnDelete(DeleteBehavior.ClientCascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}