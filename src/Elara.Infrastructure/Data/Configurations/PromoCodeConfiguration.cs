using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(pc => pc.Code)
                .IsUnique();

            builder.Property(pc => pc.DiscountValue)
                .HasColumnType("decimal(18,2)");

            builder.Property(pc => pc.MinOrderAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(pc => pc.Orders)
                .WithOne(o => o.PromoCode)
                .HasForeignKey(o => o.PromoCodeId);
        }
    }
}
