using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class ResourceStringConfiguration : IEntityTypeConfiguration<ResourceString>
    {
        public void Configure(EntityTypeBuilder<ResourceString> builder)
        {
            builder.ToTable("ResourceStrings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Key)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(r => r.Value)
                .IsRequired();

            builder.HasIndex(r => new { r.Key, r.LanguageId })
                .IsUnique();

            builder.HasOne(r => r.Language)
                .WithMany()
                .HasForeignKey(r => r.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
