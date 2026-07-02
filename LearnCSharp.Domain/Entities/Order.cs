namespace LearnCSharp.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalMoney { get; set; }
        public string ShippingMethod { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string PaymentTransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus{ get; set; }
        public string PaymentIntentId{ get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid UserId { get; set; }
        public int TotalItem { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}