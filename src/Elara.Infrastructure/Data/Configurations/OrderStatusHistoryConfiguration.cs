using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.HasKey(osh => osh.Id);

            builder.Property(osh => osh.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(osh => osh.Notes)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(osh => osh.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(osh => osh.OrderId);
        }
    }
}
