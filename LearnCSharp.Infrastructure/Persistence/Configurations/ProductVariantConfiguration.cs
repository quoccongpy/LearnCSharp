using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    internal class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("product_variant");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Price).HasColumnName("price");
            builder.Property(a => a.ProductId).HasColumnName("product_id");
            builder.Property(a => a.SizeId).HasColumnName("size_id");
            builder.Property(a => a.CrustId).HasColumnName("crust_id");
        }
    }
}