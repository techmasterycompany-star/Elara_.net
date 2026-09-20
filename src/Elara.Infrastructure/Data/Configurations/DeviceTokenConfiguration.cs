using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.ToTable("DeviceTokens");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(d => d.Token)
                .IsUnique();

            builder.Property(d => d.Platform)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(d => d.IsActive)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
