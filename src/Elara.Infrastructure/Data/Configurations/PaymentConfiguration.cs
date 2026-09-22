using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Provider)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.TransactionId)
                .IsRequired(false)
                .HasMaxLength(512);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);
        }
    }
}
