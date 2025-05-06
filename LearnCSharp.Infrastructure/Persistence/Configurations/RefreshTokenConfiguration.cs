using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_token");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.UserId).HasColumnName("user_id").HasColumnType("uuid");
            builder.Property(a => a.ExpiryDate).HasColumnName("expiry_date").HasColumnType("TIMESTAMP");
            builder.Property(a => a.IsRevoked).HasColumnName("is_revoked");
            builder.Property(a => a.CreatedDate).HasColumnName("created_date").HasColumnType("TIMESTAMP").IsRequired(false);
            builder.Property(a => a.RevokedDate).HasColumnName("revoked_date").HasColumnType("TIMESTAMP");
        }
    }
}