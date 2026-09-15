using Elara.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(EntityTypeBuilder<RevokedToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Jti)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(rt => rt.Jti)
                .IsUnique();

            builder.Property(rt => rt.RevokedAt)
                .IsRequired();

            builder.Property(rt => rt.Reason)
                .IsRequired(false)
                .HasMaxLength(500);
        }
    }
}