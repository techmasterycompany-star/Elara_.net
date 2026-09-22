using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
    {
        public void Configure(EntityTypeBuilder<ShipmentItem> builder)
        {
            builder.HasKey(si => si.Id);

            builder.HasOne(si => si.Shipment)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.ShipmentId);

            builder.HasOne(si => si.OrderItem)
                .WithMany(oi => oi.ShipmentItems)
                .HasForeignKey(si => si.OrderItemId);
        }
    }
}
