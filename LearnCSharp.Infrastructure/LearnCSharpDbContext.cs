using LearnCSharp.Domain.Entities;
using LearnCSharp.Infrastructure.Identity;
using LearnCSharp.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure
{
    public class LearnCSharpDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public LearnCSharpDbContext(DbContextOptions<LearnCSharpDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<Crust> Crust { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<ProductVariant> ProductVariant { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ProductReview> ProductReview { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies(false);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureIdentityTables();
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new OrderDetailsConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfiguration(new ProductImageConfiguration());
            builder.ApplyConfiguration(new SizeConfiguration());
            builder.ApplyConfiguration(new CrustConfiguration());
            builder.ApplyConfiguration(new ProductVariantConfiguration());
            builder.ApplyConfiguration(new NotificationConfiguration());
            builder.ApplyConfiguration(new ProductReviewConfiguration());

            base.OnModelCreating(builder);
        }
    }
}