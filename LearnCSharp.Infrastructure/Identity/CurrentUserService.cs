using LearnCSharp.Application.Interfaces;
using LearnCSharp.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid UserId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext
                                ?? throw new InvalidOperationException("No HttpContext");

                return httpContext.User.GetUserId();    
            }
        }
    }
}