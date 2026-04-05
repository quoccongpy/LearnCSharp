using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Identity;
using LearnCSharp.Infrastructure.Persistence.Repositories;
using LearnCSharp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LearnCSharp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}