namespace LearnCSharp.Infrastructure.Persistence.ConfigOptions
{
    public class JwtTokenSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireInHours { get; set; }
    }
}