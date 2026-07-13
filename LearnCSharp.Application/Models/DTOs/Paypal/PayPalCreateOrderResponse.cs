using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalCreateOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("links")]
        public List<PayPalLink> Links { get; set; } = new();
    }
    public class PayPalLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;

        [JsonPropertyName("rel")]
        public string Rel { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
    }
}