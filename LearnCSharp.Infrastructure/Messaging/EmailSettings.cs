namespace LearnCSharp.Infrastructure.Messaging
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } 
        public string Password { get; set; }  
        public string FromName { get; set; } = "LearnCSharp Shop";
        public string FromEmail { get; set; }
    }
}