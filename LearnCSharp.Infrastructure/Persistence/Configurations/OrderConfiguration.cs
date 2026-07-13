using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("order");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.UserId).HasColumnName("user_id").HasColumnType("uuid");
            builder.Property(a => a.FullName).HasColumnName("full_name").HasColumnType("TEXT");
            builder.Property(a => a.Email).HasColumnName("email").HasColumnType("TEXT");
            builder.Property(a => a.PhoneNumber).HasColumnName("phone_number").HasColumnType("TEXT");
            builder.Property(a => a.Address).HasColumnName("address").HasColumnType("TEXT");
            builder.Property(a => a.Note).HasColumnName("note").HasMaxLength(100);
            builder.Property(a => a.OrderDate).HasColumnName("ordate_date").HasColumnType("TIMESTAMP").IsRequired(false);
            builder.Property(a => a.Status).HasColumnName("status").HasMaxLength(50);
            builder.Property(a => a.TotalMoney).HasColumnName("total_money");
            builder.Property(a => a.TotalItem).HasColumnName("total_item");
            builder.Property(a => a.ShippingMethod).HasColumnName("shipping_method").HasMaxLength(100);
            builder.Property(a => a.ShippingDate).HasColumnName("shipping_date").HasColumnType("TIMESTAMP");
            builder.Property(a => a.ScheduledTime).HasColumnName("scheduled_time").HasColumnType("TIMESTAMP");
        }
    }
}