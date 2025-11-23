using BadTrip.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BadTrip.Infrastructure.Persistence.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(h => h.Id);

            builder.HasIndex(h => h.Name)
                .IsUnique();

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Stars)
                .IsRequired();

            builder.Property(h => h.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Configure Address Value Object as owned entity
            builder.OwnsOne(h => h.Address, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("Address_Street");

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Address_City");

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Address_Country");
            });
        }
    }
}
