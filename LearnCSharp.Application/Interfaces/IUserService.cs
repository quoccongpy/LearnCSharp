using LearnCSharp.Application.Models.DTOs.User;

namespace LearnCSharp.Application.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(CreateUserDTO model);

        Task<UserDTO> GetUserByIdAsync(Guid id);

        Task Update(Guid id, UpdateUserDTO model);
    }
}