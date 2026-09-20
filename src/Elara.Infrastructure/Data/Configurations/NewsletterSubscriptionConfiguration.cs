using Elara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elara.Infrastructure.Data.Configurations
{
    public class NewsletterSubscriptionConfiguration : IEntityTypeConfiguration<NewsletterSubscription>
    {
        public void Configure(EntityTypeBuilder<NewsletterSubscription> builder)
        {
            builder.ToTable("NewsletterSubscriptions");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(n => n.Email)
                .IsUnique();

            builder.Property(n => n.IsSubscribed)
                .IsRequired();

            builder.Property(n => n.SubscribedAt)
                .IsRequired();

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
