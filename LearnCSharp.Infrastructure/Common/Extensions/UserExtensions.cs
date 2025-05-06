using LearnCSharp.Infrastructure.Common.Constants;
using System.Security.Claims;

namespace LearnCSharp.Infrastructure.Common.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claims = ((ClaimsIdentity)principal.Identity).Claims.Single(a => a.Type == UserClaims.Id);
            return Guid.Parse(claims.Value);
        }
    }
}