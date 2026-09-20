using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
    {
        public void Configure(EntityTypeBuilder<Referral> builder)
        {
            builder.ToTable("Referrals");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReferralCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.ReferralCode)
                .IsUnique();

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(r => r.RewardPoints)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasOne(r => r.ReferrerUser)
                .WithMany()
                .HasForeignKey(r => r.ReferrerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReferredUser)
                .WithMany()
                .HasForeignKey(r => r.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
