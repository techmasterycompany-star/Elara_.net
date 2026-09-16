using Elara.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmation>
    {
        public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
        {
            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.UserId)
                .IsRequired();

            builder.Property(ec => ec.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(ec => ec.Token)
                .IsUnique();

            builder.Property(ec => ec.ExpiresAt)
                .IsRequired();

            builder.Property(ec => ec.IsUsed)
                .IsRequired();

            builder.HasOne(ec => ec.User)
                .WithMany()
                .HasForeignKey(ec => ec.UserId);
        }
    }
}