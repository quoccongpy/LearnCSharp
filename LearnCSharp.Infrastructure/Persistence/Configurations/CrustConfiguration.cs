using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class CrustConfiguration : IEntityTypeConfiguration<Crust>
    {
        public void Configure(EntityTypeBuilder<Crust> builder)
        {
            builder.ToTable("crust");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(350);
        }
    }
}