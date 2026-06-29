namespace LearnCSharp.Application.Models.DTOs.Payment
{
    public class PaymentIntentResultDTO
    {
        public int OrderId { get; set; }
        public string ClientSecret { get; set; }
        public string PublishableKey { get; set; }
    }
}