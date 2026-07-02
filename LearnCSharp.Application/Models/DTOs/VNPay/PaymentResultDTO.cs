namespace LearnCSharp.Application.Models.DTOs.VNPay
{
    public class PaymentResultDTO
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public long OrderId { get; set; }
        public string VnPayResponseCode { get; set; }

        public string PaymentMethod { get; set; }

        public string TransactionId { get; set; }
        public string OrderDescription { get; set; }
        public decimal Amount { get; set; }
    }
}