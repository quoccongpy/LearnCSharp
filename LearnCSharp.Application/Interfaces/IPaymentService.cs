using LearnCSharp.Application.Models.DTOs.Payment;

namespace LearnCSharp.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentResultDTO> CreatePaymentIntentAsync(int orderId);

        Task HandleWebhookAsync(string json, string stripeSignature);
    }
}