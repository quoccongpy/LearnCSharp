namespace LearnCSharp.Application.Models.DTOs.User
{
    public class UpdateUserDTO
    {
        //public Guid Id { get; set; }
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}