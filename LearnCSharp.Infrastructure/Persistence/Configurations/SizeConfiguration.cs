using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.ToTable("size");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(350);
        }
    }
}