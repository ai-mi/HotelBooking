using HotelBooking.EndPoint.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Data;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<LoyaltyMember> LoyaltyMembers { get; set; }
    public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
    public DbSet<RoomAuditLog> RoomAuditLogs { get; set; }
    public DbSet<BookingAuditLog> BookingAuditLogs { get; set; }
    public DbSet<CustomerAuditLog> CustomerAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hotel Configuration
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.HasIndex(e => e.Name);
        });

        // Room Configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.HotelId, e.RoomNumber }).IsUnique();
        });

        // Customer Configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Booking Configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.RoomId, e.CheckInDate, e.CheckOutDate });
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(e => e.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // LoyaltyMember Configuration
        modelBuilder.Entity<LoyaltyMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MembershipNumber).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Customer)
                .WithOne(c => c.LoyaltyMember)
                .HasForeignKey<LoyaltyMember>(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.MembershipNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId).IsUnique();
        });

        // LoyaltyTransaction Configuration
        modelBuilder.Entity<LoyaltyTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.LoyaltyMember)
                .WithMany(m => m.Transactions)
                .HasForeignKey(e => e.LoyaltyMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.LoyaltyMemberId);
        });

        // RoomAuditLog Configuration
        modelBuilder.Entity<RoomAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.AuditLogs)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.RoomId, e.Timestamp });
        });

        // BookingAuditLog Configuration
        modelBuilder.Entity<BookingAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);

            entity.HasOne(e => e.Booking)
                .WithMany(b => b.AuditLogs)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.BookingId, e.Timestamp });
        });

        // CustomerAuditLog Configuration
        modelBuilder.Entity<CustomerAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.PerformedBy).HasMaxLength(200);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.AuditLogs)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.CustomerId, e.Timestamp });
        });
    }
}
