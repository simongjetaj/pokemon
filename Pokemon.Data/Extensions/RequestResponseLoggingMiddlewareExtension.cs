using Microsoft.AspNetCore.Builder;
using Pokemon.Data.Middlewares;

namespace Pokemon.Data.Extensions
{
    public static class RequestResponseLoggingMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
