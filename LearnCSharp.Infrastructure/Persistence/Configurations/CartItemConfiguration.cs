using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("cart_item");
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id).HasColumnName("id");
            builder.Property(ci => ci.CartId).HasColumnName("cart_id");
            builder.Property(ci => ci.ProductId).HasColumnName("product_id");
            builder.Property(ci => ci.ProductVariantId).HasColumnName("product_variant_id").IsRequired(false);
            builder.Property(ci => ci.Quantity).HasColumnName("quantity");
            builder.Property(ci => ci.Note).HasColumnName("note").HasMaxLength(72);
            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(ci => new { ci.CartId, ci.ProductId, ci.ProductVariantId, ci.Note }).IsUnique();
        }
    }
}