using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.User)
                .WithMany(u => u.TransactionTypes)
                .HasForeignKey(t => t.UserId);

            builder.HasOne(t => t.Order)
                .WithMany(o => o.LoyaltyTransactions)
                .HasForeignKey(t => t.OrderId);
        }
    }
}
