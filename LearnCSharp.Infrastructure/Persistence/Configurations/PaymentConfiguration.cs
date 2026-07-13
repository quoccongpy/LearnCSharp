using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.OrderId).HasColumnName("order_id");
            builder.Property(a => a.PaymentMethod).HasColumnName("payment_method").HasMaxLength(100);
            builder.Property(a => a.PaymentStatus).HasColumnName("payment_status").HasMaxLength(20);
            builder.Property(a => a.Amount).HasColumnName("amount");
            builder.Property(a => a.PaymentDate).HasColumnName("payment_date").HasColumnType("TIMESTAMP");
            builder.Property(a => a.GatewayOrderId).HasColumnName("gateway_order_id").HasMaxLength(100);
            builder.Property(a => a.GatewayTransactionId).HasColumnName("gateway_transaction_id").HasMaxLength(100);
            builder.Property(a => a.GatewayMetadata).HasColumnName("gateway_metadata").HasMaxLength(-1);
        }
    }
}