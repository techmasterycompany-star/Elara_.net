using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
    {
        public void Configure(EntityTypeBuilder<ShippingMethod> builder)
        {
            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sm => sm.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sm => sm.BaseCost)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(sm => sm.Orders)
                .WithOne(o => o.ShippingMethod)
                .HasForeignKey(o => o.ShippingMethodId);
        }
    }
}
