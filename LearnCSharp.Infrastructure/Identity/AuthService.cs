using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Auth;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using LearnCSharp.Domain.Entities;
using System.Security.Cryptography;

namespace LearnCSharp.Infrastructure.Identity
{
    //https://www.c-sharpcorner.com/article/jwt-authentication-with-refresh-tokens-in-net-6-0/
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (!user.IsActive)
            {
                throw new Exception("Account has been locked");
            }
            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.RefreshToken.CreateAsync(refreshToken);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
                var data = new AuthDTO()
                {
                    AccessToken = token,
                    RefreshToken= refreshToken.Token,
                    ExpiresAt =refreshToken.ExpiryDate,
                };
                return data;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<AuthDTO> RefreshTokenAsync(RefreshTokenDTO model)
        {
            try
            {
                var tokenHandle = new JwtSecurityTokenHandler();
                var principal = GetPrincipalFromExpiredToken(model.AccessToken);
                if (principal == null)
                {
                    throw new Exception("Access is Invalid");
                }
                var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid user claim");
                }
                var user = await _userManager.FindByIdAsync(userIdString);
                if (user == null || !user.IsActive)
                {
                    throw new Exception("User does not exist or is locked");
                }
                var refreshTokenFromDb = await GetRefreshTokenAsync(model.RefreshToken);
                if (refreshTokenFromDb == null ||refreshTokenFromDb.UserId != user.Id ||
                    refreshTokenFromDb.ExpiryDate < DateTime.UtcNow || refreshTokenFromDb.IsRevoked)
                {
                    throw new Exception("Refresh token is invalid or expired");
                }
                var newToken = await GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken(user.Id);
                await _unitOfWork.BeginTransactionAsync();

                refreshTokenFromDb.IsRevoked = true;
                refreshTokenFromDb.RevokedDate = DateTime.Now;
                _unitOfWork.RefreshToken.Update(refreshTokenFromDb);

                await _unitOfWork.RefreshToken.CreateAsync(newRefreshToken);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                var data = new AuthDTO()
                {
                    AccessToken = newToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiresAt = newRefreshToken.ExpiryDate,
                };
                return data;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtTokenSettings.Issuer,
                ValidAudience = _jwtTokenSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.Key)),
                ValidateLifetime = false 
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var timeExpire = DateTime.Now.AddHours(_jwtTokenSettings.ExpireInHours);

            var listClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.FullName),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                listClaims.Add(new Claim(ClaimTypes.Role,role));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtTokenSettings.Issuer,
                audience: _jwtTokenSettings.Audience,
                claims: listClaims,
                expires: timeExpire,
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            var randomNumber = new Byte[32];
            using(var rng = RandomNumberGenerator.Create()) 
            {
                rng.GetBytes(randomNumber);
            }
            var tokenString = Convert.ToBase64String(randomNumber);

            var data = new RefreshToken()
            {
                Token = tokenString,
                UserId = userId,
                ExpiryDate = DateTime.Now.AddDays(7),
                CreatedDate = DateTime.Now,
                IsRevoked = false,
            };
            return data;
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string tokenValue)
        {
            var data = await _unitOfWork.RefreshToken.GetByfilterAsync(a=>a.Token==tokenValue);
            return data;

        }
       
    }
}