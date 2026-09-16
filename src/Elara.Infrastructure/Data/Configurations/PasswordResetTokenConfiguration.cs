using Elara.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(prt => prt.Id);

            builder.Property(prt => prt.UserId)
                .IsRequired();

            builder.Property(prt => prt.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(prt => prt.Token)
                .IsUnique();

            builder.Property(prt => prt.ExpiresAt)
                .IsRequired();

            builder.Property(prt => prt.IsUsed)
                .IsRequired();

            builder.HasOne(prt => prt.User)
                .WithMany()
                .HasForeignKey(prt => prt.UserId);
        }
    }
}