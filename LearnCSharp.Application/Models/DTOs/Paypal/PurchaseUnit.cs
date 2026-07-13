using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PurchaseUnit
    {
        [JsonPropertyName("reference_id")]
        public string ReferenceId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("amount")]
        public Amount Amount { get; set; }
    }

    public class Amount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}