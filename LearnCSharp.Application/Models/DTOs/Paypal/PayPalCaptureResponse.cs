using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalCaptureResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("purchase_units")]
        public List<PayPalCapturePurchaseUnit> PurchaseUnits { get; set; } = new();
    }

    public class PayPalCapturePurchaseUnit
    {
        [JsonPropertyName("payments")]
        public PayPalCapturePayments? Payments { get; set; }
    }

    public class PayPalCapturePayments
    {
        [JsonPropertyName("captures")]
        public List<PayPalCapture> Captures { get; set; } = new();
    }

    public class PayPalCapture
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public PayPalMoney? Amount { get; set; }
    }

    public class PayPalMoney
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
}