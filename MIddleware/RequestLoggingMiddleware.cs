namespace Mylearningapi.Middleware
{
    public class RequestLoggingMiddleware
    {
        // RequestDelegate is a type that represents a function which can process an HTTP request.
        // In ASP.NET Core, the request pipeline is a chain of delegates (middleware). 
        // Each one can handle the request and optionally pass it to the next.
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");
            await _next(context);
            Console.WriteLine($"[Response] Status: {context.Response.StatusCode}");
        }
    }
}