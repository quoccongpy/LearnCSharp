using LearnCSharp.API.Middlewares;

namespace LearnCSharp.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}