using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Identity;
using LearnCSharp.Infrastructure.Messaging;
using LearnCSharp.Infrastructure.Persistence.Repositories;
using LearnCSharp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using LearnCSharp.Infrastructure.Messaging.Consumers;

namespace LearnCSharp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<ICrustService, CrustService>();
            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddHttpClient<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IPayPalService, PayPalService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            var rabbitSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();
                x.AddConsumer<OrderStatusChangedConsumer>();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitSettings.Username);
                        h.Password(rabbitSettings.Password);
                    });
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            return services;
        }
    }
}