using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Auth;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LearnCSharp.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenSettings _jwtTokenSettings;

        public AuthService(IConfiguration configuration,
                           UserManager<AppUser> userManager,
                           IUnitOfWork unitOfWork,
                           IOptions<JwtTokenSettings> jwtTokenSettings)
        {
            _configuration = configuration;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtTokenSettings = jwtTokenSettings.Value;
        }

        public async Task<AuthDTO> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (!user.IsActive)
            {
                throw new Exception("Account has been locked");
            }
            var tokenExpire = DateTime.Now.AddHours(_jwtTokenSettings.ExpireInHours);
        }

        private async Task<string> GenerateJwtToken(App)
    }
}