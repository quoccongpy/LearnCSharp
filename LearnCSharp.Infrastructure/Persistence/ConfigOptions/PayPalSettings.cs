namespace LearnCSharp.Infrastructure.Persistence.ConfigOptions
{
    public class PayPalSettings
    {
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string Mode { get; set; } 
        public string Currency { get; set; } 
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string BaseUrl { get; set; }
    }
}