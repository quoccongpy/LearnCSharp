using LearnCSharp.Infrastructure.Common.Constants;
using System.Security.Claims;

namespace LearnCSharp.Infrastructure.Common.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId!);
        }
    }
}