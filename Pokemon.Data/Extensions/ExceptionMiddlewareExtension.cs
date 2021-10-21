using Microsoft.AspNetCore.Builder;
using Pokemon.Data.Middlewares;

namespace Pokemon.Data.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
