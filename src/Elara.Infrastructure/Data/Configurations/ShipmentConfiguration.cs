using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Carrier)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasOne(s => s.Order)
                .WithMany(o => o.Shipments)
                .HasForeignKey(s => s.OrderId);

            builder.HasOne(s => s.SellerProfile)
                .WithMany(sp => sp.Shipments)
                .HasForeignKey(s => s.SellerProfileId);

            builder.HasMany(s => s.Items)
                .WithOne(si => si.Shipment)
                .HasForeignKey(si => si.ShipmentId);
        }
    }
}
