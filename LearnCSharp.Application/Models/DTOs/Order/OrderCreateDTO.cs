namespace LearnCSharp.Application.Models.DTOs.Order
{
    public class OrderCreateDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string ShippingMethod { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }
}