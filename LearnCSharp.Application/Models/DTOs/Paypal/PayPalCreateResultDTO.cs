namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalCreateResultDTO
    {
        public int OrderId { get; set; }
        public string PayPalOrderId { get; set; }
        public string ApprovalUrl { get; set; }
    }
}