using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(350);
            builder.Property(a => a.Price).HasColumnName("price");
            builder.Property(a => a.Thumbnail).HasColumnName("thumbnail").HasMaxLength(300);
            builder.Property(a => a.Description).HasColumnName("description").HasColumnType("TEXT");
            builder.Property(a => a.CreatedDate).HasColumnName("create_at").HasColumnType("TIMESTAMP").IsRequired(false);
            builder.Property(a => a.UpdatedDate).HasColumnName("update_at").HasColumnType("TIMESTAMP").IsRequired(false);
            builder.Property(a => a.CategoryId).HasColumnName("category_id");
        }
    }
}