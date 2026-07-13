using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalCreateOrderRequest
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; }
        [JsonPropertyName("purchase_units")]
        public List<PurchaseUnit> PurchaseUnits { get; set; }
        [JsonPropertyName("application_context")]
        public PayPalApplicationContext ApplicationContext { get; set; }
    }
}