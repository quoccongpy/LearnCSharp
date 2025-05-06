using LearnCSharp.Application.Models.DTOs.User;

namespace LearnCSharp.Application.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(CreateUserDTO model);

        Task<UserDTO> GetUserByIdAsync(Guid id);

        Task UpdateAsync(Guid id, UpdateUserDTO model);

        Task DeleteAsync(Guid id, bool isActive);

        Task ChangePasswordAsync(Guid id, ChangePasswordDTO model);

        Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    }
}