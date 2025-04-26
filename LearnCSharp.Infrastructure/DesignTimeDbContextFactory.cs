using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LearnCSharp.Infrastructure
{
    //https://gist.github.com/ShawnShiSS/c32b3e253804ca6c3b37c70b1ce0f36d
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LearnCSharpDbContext>
    {
        public LearnCSharpDbContext CreateDbContext(string[] args)
        {
            // Load cấu hình từ appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<LearnCSharpDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new LearnCSharpDbContext(builder.Options);
        }
    }
}