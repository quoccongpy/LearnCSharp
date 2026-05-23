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
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Price).HasColumnName("price");
            builder.Property(a => a.ProductId).HasColumnName("product_id");
            builder.Property(a => a.SizeId).HasColumnName("size_id");
            builder.Property(a => a.CrustId).HasColumnName("crust_id");

            builder.HasOne(a => a.Product)
                   .WithMany(p => p.ProductVariants)
                   .HasForeignKey(a => a.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Size)
                   .WithMany(s => s.ProductVariants)
                   .HasForeignKey(a => a.SizeId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Crust)
                   .WithMany(c => c.ProductVariants)
                   .HasForeignKey(a => a.CrustId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.ProductId, a.SizeId, a.CrustId }).IsUnique();
        }
    }
}