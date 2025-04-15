using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure.Persistence.Configurations
{
    public static class IdentityConfiguration
    {
        public static void ConfigureIdentityTables(this ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(a => a.Id);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(a => a.Id);
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(a => a.UserId);
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(a => new { a.RoleId, a.UserId });
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(a => new { a.UserId });
        }
    }
}