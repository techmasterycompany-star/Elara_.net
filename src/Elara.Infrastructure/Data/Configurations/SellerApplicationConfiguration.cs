using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class SellerApplicationConfiguration : IEntityTypeConfiguration<SellerApplication>
    {
        public void Configure(EntityTypeBuilder<SellerApplication> builder)
        {
            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.StoreName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(sa => sa.StoreDescription)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(sa => sa.User)
                .WithMany(u => u.SellerApplications)
                .HasForeignKey(sa => sa.UserId);

            builder.HasIndex(x => x.UserId)
                .HasFilter("[Status] = 1")
                .IsUnique();
        }
    }
}
