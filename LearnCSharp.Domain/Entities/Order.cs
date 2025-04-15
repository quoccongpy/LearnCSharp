using LearnCSharp.Domain.Enums;

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
        public OrderStatus Status { get; set; }
        public float TotalMoney { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime ShippingDate { get; set; }
        public string TrackingNumber { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
    }
}