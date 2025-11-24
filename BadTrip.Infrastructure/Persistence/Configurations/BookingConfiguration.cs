using BadTrip.Domain.Entities;
using BadTrip.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BadTrip.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.TourId)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();

        // Configure TotalPrice as owned entity (Value Object)
        builder.OwnsOne(b => b.TotalPrice, price =>
        {
            price.Property(p => p.Amount)
                .IsRequired()
                .HasColumnName("TotalPrice_Amount")
                .HasColumnType("decimal(18,2)");

            price.Property(p => p.Currency)
                .IsRequired()
                .HasColumnName("TotalPrice_Currency")
                .HasMaxLength(3);
        });

        // Configure Passengers as owned collection in separate table
        builder.OwnsMany(b => b.Passengers, passenger =>
        {
            passenger.ToTable("Booking_Passengers");

            passenger.WithOwner()
                .HasForeignKey("BookingId");

            passenger.HasKey("BookingId", "PassportNumber");

            passenger.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            passenger.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            passenger.Property(p => p.PassportNumber)
                .IsRequired()
                .HasMaxLength(50);

            passenger.Property(p => p.DateOfBirth)
                .IsRequired();
        });

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Tour)
            .WithMany()
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique index: One active booking per user per tour
        // Only non-cancelled bookings count toward uniqueness
        builder.HasIndex(b => new { b.UserId, b.TourId, b.Status })
            .HasFilter("[Status] != 'Cancelled'")
            .IsUnique()
            .HasDatabaseName("IX_Bookings_UserId_TourId_Status_Unique");

        // Additional indexes for query performance
        builder.HasIndex(b => b.UserId)
            .HasDatabaseName("IX_Bookings_UserId");

        builder.HasIndex(b => b.TourId)
            .HasDatabaseName("IX_Bookings_TourId");

        builder.HasIndex(b => b.Status)
            .HasDatabaseName("IX_Bookings_Status");
    }
}
