namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalCaptureResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}