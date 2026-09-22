using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("Banners");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.Subtitle)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.ImagePublicId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.LinkUrl)
                .HasMaxLength(500);

            builder.Property(b => b.Position)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(b => b.DisplayOrder)
                .IsRequired();

            builder.Property(b => b.IsActive)
                .IsRequired();

            builder.Property(b => b.StartDate);

            builder.Property(b => b.EndDate);

            builder.HasIndex(b => new
            {
                b.IsActive,
                b.Position,
                b.DisplayOrder
            });
        }
    }
}
