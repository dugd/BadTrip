using BadTrip.Domain.Entities;
using BadTrip.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BadTrip.Infrastructure.Persistence.Configurations
{
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(t => t.Title)
                .IsUnique();

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(t => t.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Configure Money as owned entity (embedded in same table)
            builder.OwnsOne(t => t.Price, price =>
            {
                price.Property(m => m.Amount)
                    .HasColumnName("Price_Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(m => m.Currency)
                    .HasColumnName("Price_Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(t => t.HotelId)
                .IsRequired(false);

            builder.Property(t => t.MaxParticipants)
                .IsRequired();

            builder.Property(t => t.SoldSpots)
                .IsRequired();

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.EndDate)
                .IsRequired();

            builder.Property(t => t.OperatorId)
                .IsRequired();

            // Optimistic concurrency control
            builder.Property(t => t.RowVersion)
                .IsRowVersion();

            // Relationships
            builder.HasOne(t => t.Hotel)
                .WithMany()
                .HasForeignKey(t => t.HotelId)
                .OnDelete(DeleteBehavior.SetNull); // If hotel is deleted, tour.HotelId becomes null

            builder.HasOne(t => t.Operator)
                .WithMany()
                .HasForeignKey(t => t.OperatorId)
                .OnDelete(DeleteBehavior.Restrict); // Cannot delete operator if they have tours

            // Indexes for common queries
            builder.HasIndex(t => t.OperatorId);
            builder.HasIndex(t => t.StartDate);
            builder.HasIndex(t => t.HotelId);
        }
    }
}
