namespace LearnCSharp.Infrastructure.Persistence.ConfigOptions
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublicableKey { get; set; }
        public string WebhookSecret { get; set; }
        public string Currency { get; set; } = "usd";
    }
}