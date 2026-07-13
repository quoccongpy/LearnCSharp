namespace LearnCSharp.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public decimal Amount { get; set; }

        public DateTime? PaymentDate { get; set; } = DateTime.UtcNow;

        public string GatewayOrderId { get; set; }

        public string GatewayTransactionId { get; set; }

        public string GatewayMetadata { get; set; }
    }
}