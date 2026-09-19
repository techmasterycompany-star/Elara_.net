using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class HomepageSectionConfiguration : IEntityTypeConfiguration<HomepageSection>
    {
        public void Configure(EntityTypeBuilder<HomepageSection> builder)
        {
            builder.ToTable("HomepageSections");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.SubTitle)
                .HasMaxLength(255);

            builder.Property(s => s.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(s => s.DisplayOrder)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.Property(s => s.MaxItems)
                .IsRequired();

            builder.HasOne(s => s.Banner)
                .WithMany(b => b.HomepageSections)
                .HasForeignKey(s => s.BannerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.Category)
                .WithMany(c => c.HomepageSections)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(s => new
            {
                s.IsActive,
                s.DisplayOrder
            });
        }
    }
}
