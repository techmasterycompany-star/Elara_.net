using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Discount)
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            builder.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            builder.HasMany(oi => oi.ShipmentItems)
                .WithOne(si => si.OrderItem)
                .HasForeignKey(si => si.OrderItemId);
        }
    }
}
