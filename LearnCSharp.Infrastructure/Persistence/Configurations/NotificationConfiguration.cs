using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notification");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.UserId).HasColumnName("user_id").HasColumnType("uuid");
            builder.Property(a => a.Title).HasColumnName("title").HasMaxLength(200);
            builder.Property(a => a.Message).HasColumnName("message").HasColumnType("TEXT");
            builder.Property(a => a.Type).HasColumnName("type").HasMaxLength(20);
            builder.Property(a => a.OrderId).HasColumnName("order_id").IsRequired(false);
            builder.Property(a => a.IsRead).HasColumnName("is_read").HasDefaultValue(false);
            builder.Property(a => a.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP");

            builder.HasIndex(a => new { a.UserId, a.IsRead });
        }
    }
}