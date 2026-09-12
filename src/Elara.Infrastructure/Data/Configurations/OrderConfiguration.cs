using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.GuestFullName)
                .HasMaxLength(150);

            builder.Property(o => o.GuestEmail)
                .HasMaxLength(256);

            builder.Property(o => o.GuestPhoneNumber)
                .HasMaxLength(20);

            builder.Property(o => o.ShippingFullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.ShippingPhone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.ShippingStreet)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.ShippingState)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.ShippingPostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.ShippingCountry)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            builder.HasOne(o => o.ShippingMethod)
                .WithMany(sm => sm.Orders)
                .HasForeignKey(o => o.ShippingMethodId);

            builder.HasOne(o => o.PromoCode)
                .WithMany(pc => pc.Orders)
                .HasForeignKey(o => o.PromoCodeId);

            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            builder.HasMany(o => o.StatusHistory)
                .WithOne(osh => osh.Order)
                .HasForeignKey(osh => osh.OrderId);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);

            builder.HasMany(o => o.Shipments)
                .WithOne(s => s.Order)
                .HasForeignKey(s => s.OrderId);

            builder.HasMany(o => o.LoyaltyTransactions)
                .WithOne(t => t.Order)
                .HasForeignKey(t => t.OrderId);
        }
    }
}
