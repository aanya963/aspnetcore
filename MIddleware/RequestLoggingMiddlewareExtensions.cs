using Microsoft.AspNetCore.Builder;
using Mylearningapi.Middleware;

namespace Mylearningapi.Middleware
{
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}