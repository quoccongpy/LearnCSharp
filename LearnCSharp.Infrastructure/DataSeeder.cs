using LearnCSharp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace LearnCSharp.Infrastructure
{
    public class DataSeeder
    {
        public async Task SeedAsync(LearnCSharpDbContext context)
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            var adminRoleId = Guid.NewGuid();
            if (!context.Roles.Any())
            {
                var data = new AppRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                };
                await context.Roles.AddAsync(data);
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var userId = Guid.NewGuid();
                var user = new AppUser()
                {
                    Id = userId,
                    FullName = "admin",
                    Email = "admin@gmail",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                    CreatedDate = DateTime.UtcNow,
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "Admin@@123$");
                await context.Users.AddAsync(user);

                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>()
                {
                    RoleId = adminRoleId,
                    UserId = userId,
                });
                await context.SaveChangesAsync();
            }
        }
    }
}