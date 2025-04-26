using LearnCSharp.Application.Models.DTOs.Auth;

namespace LearnCSharp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDTO> LoginAsync(LoginDTO model);
        Task<AuthDTO> RefreshTokenAsync(RefreshTokenDTO model);
    }
}