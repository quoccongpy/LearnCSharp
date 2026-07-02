namespace LearnCSharp.Application.Models.DTOs.VNPay
{
    public class IpnResponseDTO
    {
        public string RspCode { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
