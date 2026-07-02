using LearnCSharp.Application.Models.DTOs.VNPay;

namespace LearnCSharp.Application.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(int orderId, string ipAddress);

        PaymentResultDTO PaymentExecute(Dictionary<string, string> collections);

        Task<IpnResponseDTO> ProcessIpnAsync(Dictionary<string, string> query);
    }
}