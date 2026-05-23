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
            builder.Property(a => a.Price).HasColumnName("price");
            builder.Property(a => a.Quantity).HasColumnName("quantity");
            builder.Property(a => a.Total).HasColumnName("total");
            builder.Property(a => a.ProductVariantId).HasColumnName("product_variant_id").IsRequired(false);
            //builder.Property(a => a.Color).HasColumnName("color").HasMaxLength(20);
        }
    }
}