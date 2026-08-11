using LearnCSharp.API;
using LearnCSharp.API.Extensions;
using LearnCSharp.API.Hubs;
using LearnCSharp.API.Services;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Infrastructure;
using LearnCSharp.Infrastructure.Identity;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var LearnCSharpCorsPolicy = "LearnCSharpCorsPolicy";
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContext<LearnCSharpDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o => o.AddPolicy(LearnCSharpCorsPolicy, builder =>
{
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(configuration["AllowedOrigins"])
        .AllowCredentials();
}));

//redis cache
var cacheConfig = builder.Configuration.GetSection("Cache");
var redisConnection = cacheConfig["RedisConnection"];
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddInfrastructure();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
Stripe.StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];
builder.Services.Configure<VnPaySettings>(builder.Configuration.GetSection("VNPaySettings"));
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PaypalSettings"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<LearnCSharpDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JwtTokenSettings"));

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtTokenSettings:Key"])),
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JwtTokenSettings:Issuer"],
    ValidateAudience = true,
    ValidAudience = builder.Configuration["JwtTokenSettings:Audience"],
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = tokenValidationParameters;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
        apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);

    c.SwaggerDoc("AdminAPI", new OpenApiInfo
    {
        Version = "v1",
        Title = "API",
        Description = "API for Ecommerce"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT token here (only the token, no 'Bearer ' needed)",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/AdminAPI/swagger.json", "Admin API");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseGlobalExceptionHandling();
app.UseCors(LearnCSharpCorsPolicy);
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notification").RequireCors(LearnCSharpCorsPolicy);

app.MapControllers();

app.MigrateDatabase();

app.Run();