namespace LearnCSharp.Application.Models.DTOs.Auth
{
    public class AuthDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}