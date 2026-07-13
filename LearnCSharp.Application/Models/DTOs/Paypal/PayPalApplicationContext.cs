using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalApplicationContext
    {
        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public string BrandName { get; set; }

        public string LandingPage { get; set; }

        public string UserAction { get; set; }
    }
}