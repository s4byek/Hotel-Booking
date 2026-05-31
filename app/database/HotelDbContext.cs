using Microsoft.EntityFrameworkCore;
using hotel_booking.model;

namespace hotel_booking.database;

public class HotelDbContext(DbContextOptions<HotelDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.NightlyRate).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.GuestName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.CheckInDate).HasColumnType("date");
            entity.Property(x => x.CheckOutDate).HasColumnType("date");
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.Property(x => x.RoomIds)
                .HasColumnType("uuid[]")
                .IsRequired();
        });
    }
}
