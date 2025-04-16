using LearnCSharp.Domain.Entities;
using LearnCSharp.Infrastructure.Identity;
using LearnCSharp.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure
{
    public class LearnCSharpDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public LearnCSharpDbContext(DbContextOptions<LearnCSharpDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureIdentityTables();
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new OrderDetailsConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfiguration(new ProductImageConfiguration());

            base.OnModelCreating(builder);
        }
    }
}