using Elara.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.Expires)
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(rt => rt.ReplacedByToken)
                .IsRequired(false)
                .HasMaxLength(256);

            builder.HasIndex(rt => rt.UserId);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
        }
    }
}