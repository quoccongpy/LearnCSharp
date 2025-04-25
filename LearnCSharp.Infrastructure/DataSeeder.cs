using LearnCSharp.Application.Utility;
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
                var roles = new List<AppRole>
                {
                    new AppRole
                    {
                        Id= adminRoleId,
                        Name = SD.RoleAdmin,
                        NormalizedName = "ADMIN",
                    },
                    new AppRole
                    {
                        Id= Guid.NewGuid(),
                        Name = SD.RoleCustomer,
                        NormalizedName = "CUSTOMER",
                    }
                };
                foreach (var role in roles)
                {
                    await context.Roles.AddAsync(role);
                    await context.SaveChangesAsync();
                }
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