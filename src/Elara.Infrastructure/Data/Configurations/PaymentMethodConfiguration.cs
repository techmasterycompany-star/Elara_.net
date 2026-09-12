using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Provider)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pm => pm.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(pm => pm.Last4Digits)
                .IsRequired()
                .HasMaxLength(4);

            builder.HasOne(pm => pm.User)
                .WithMany(u => u.PaymentMethods)
                .HasForeignKey(pm => pm.UserId);
        }
    }
}
