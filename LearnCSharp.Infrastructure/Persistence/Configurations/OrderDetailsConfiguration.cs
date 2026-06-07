using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            builder.ToTable("order_details");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.OrderId).HasColumnName("order_id");
            builder.Property(a => a.ProductId).HasColumnName("product_id");
            builder.Property(a => a.Quantity).HasColumnName("quantity");
            builder.Property(a => a.Total).HasColumnName("total");
            builder.Property(a => a.ProductVariantId).HasColumnName("product_variant_id").IsRequired(false);
            builder.Property(a => a.ProductName).HasColumnName("product_name").HasMaxLength(100).IsRequired();
            builder.Property(a => a.SizeName).HasColumnName("size_name").HasMaxLength(100).IsRequired(false);
            builder.Property(a => a.CrustName).HasColumnName("crust_name").HasMaxLength(100).IsRequired(false);
            builder.Property(a => a.Note).HasColumnName("note").HasMaxLength(250).IsRequired(false);
            builder.Property(a => a.UnitPrice).HasColumnName("unit_price").IsRequired();
            //builder.Property(a => a.Color).HasColumnName("color").HasMaxLength(20);
        }
    }
}