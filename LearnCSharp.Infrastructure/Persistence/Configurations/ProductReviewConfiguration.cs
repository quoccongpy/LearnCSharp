using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("product_review");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);
            builder.Property(r => r.IsHidden).HasDefaultValue(false);
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
            builder.HasOne(r => r.Product).WithMany(p => p.Reviews).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}