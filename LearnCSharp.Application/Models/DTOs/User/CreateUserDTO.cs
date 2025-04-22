namespace LearnCSharp.Application.Models.DTOs.User
{
    public class CreateUserDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
    }
}