using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
    {
        public void Configure(EntityTypeBuilder<SellerProfile> builder)
        {
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.StoreName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(sp => sp.StoreDescription)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasIndex(sp => sp.UserId)
                .IsUnique();

            builder.HasOne(sp => sp.User)
                .WithOne(u => u.SellerProfile)
                .HasForeignKey<SellerProfile>(sp => sp.UserId);

            builder.HasMany(sp => sp.Products)
                .WithOne(p => p.SellerProfile)
                .HasForeignKey(p => p.SellerProfileId);

            builder.HasMany(sp => sp.Shipments)
                .WithOne(s => s.SellerProfile)
                .HasForeignKey(s => s.SellerProfileId);

            builder.HasMany(sp => sp.Payouts)
                .WithOne(p => p.SellerProfile)
                .HasForeignKey(p => p.SellerProfileId);
        }
    }
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
        }
    }
}
