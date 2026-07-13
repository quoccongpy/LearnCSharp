using System.Text.Json.Serialization;

namespace LearnCSharp.Application.Models.DTOs.Paypal
{
    public class PayPalTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}