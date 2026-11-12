using Microsoft.EntityFrameworkCore;
using ReservasHotel.Api.Entities;

namespace ReservasHotel.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(r => r.Number).IsRequired().HasMaxLength(10);
            entity.HasIndex(r => r.Number).IsUnique();
            entity.Property(r => r.PricePerNight).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(r => r.TotalPrice).HasPrecision(18, 2);

            entity.HasOne(r => r.User)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Room)
                  .WithMany(room => room.Reservations)
                  .HasForeignKey(r => r.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}