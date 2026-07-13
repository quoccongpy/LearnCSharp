using LearnCSharp.Application.Models.DTOs.Paypal;

namespace LearnCSharp.Application.Interfaces
{
    public interface IPayPalService
    {
        Task<PayPalCreateResultDTO> CreatePayPalOrderAsync(int orderId);
        Task<PayPalCaptureResultDTO> CapturePaymentAsync(string paypalOrderId);
    }
}