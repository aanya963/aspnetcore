# 1. MIDDLEWARE : 
    Middleware is software that's assembled into the application pipeline to handle requests and responses.Each componenet in the middleware is responsible for invoking the next component in the sequence or short-circuiting the chain if neccesary. Middleware components can perform a variety of tasks, such as authentication, routing, logging, and response compression.

    -----------------------------------------------------
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    // Middleware 1: Logging the incoming request path
    app.Use(async (context, next) =>
    {
        Console.WriteLine($"[Middleware 1] Request: {context.Request.Method} {context.Request.Path}");
        await next(); // Call the next middleware in the pipeline
        Console.WriteLine("[Middleware 1] Response status: " + context.Response.StatusCode);
    });

    // Middleware 2: Timing how long the request takes
    app.Use(async (context, next) =>
    {
        var start = DateTime.UtcNow;
        await next(); // call the next middleware
        var elapsed = DateTime.UtcNow - start;
        Console.WriteLine($"[Middleware 2] Request took {elapsed.TotalMilliseconds} ms");
    });

    // Terminal Middleware: This handles the request and sends the response
    app.Run(async context =>
    {
        Console.WriteLine("[Endpoint] Handling request and generating response");
        await context.Response.WriteAsync("Hello from the endpoint!");
    });

    app.Run();

    -------------------------------------------------------------------
BUILT-IN MIDDLEWARE : 

    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    // 1. Developer exception page (shows detailed errors in dev)
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // 2. Serving static files from wwwroot
    app.UseStaticFiles();

    // 3. Routing middleware
    app.UseRouting();

    // 4. Authorization middleware (checks user permissions)
    app.UseAuthorization();

    // 5. Define endpoints
    app.MapGet("/", () => "Home page");
    app.MapGet("/about", () => "About page");

    app.Run();
    -----------------------------------------------------------

CUSTOM MIDDLEWARE : 

    To create custom middleware in ASP.NET Core, you define a class with an Invoke or InvokeAsync method that takes HttpContext as a parameter and returns a Task. The class may also include a constructor that accepts any required dependencies. This custom middleware is then registered in the application’s request pipeline, typically in the Configure method of the Startup class, using the UseMiddleware<TMiddleware> extension method.

1. Custom Middleware Class

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        // Constructor: ASP.NET Core injects the "next" delegate
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // InvokeAsync: this is where your middleware logic lives
        public async Task InvokeAsync(HttpContext context)
        {
            // Code that runs BEFORE the next middleware
            Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");

            // Call the next middleware in the pipeline
            await _next(context);

            // Code that runs AFTER the next middleware
            Console.WriteLine($"[Response] Status: {context.Response.StatusCode}");
        }
    }

2. Optional: Extension Method for Cleaner Registration

    using Microsoft.AspNetCore.Builder;

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }

3. Program.cs registration (app.UseRequestLogging())

    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    // Use your custom middleware
    app.UseRequestLogging();
    // or: app.UseMiddleware<RequestLoggingMiddleware>();

    // Example endpoints
    app.MapGet("/", () => "Hello World");
    app.MapGet("/test", () => "Test endpoint");

    app.Run();

4. Registering Middleware (Startup.cs style, .NET 5 and earlier pattern)
Startup.cs:
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use your custom middleware
            app.UseRequestLogging();
            // or: app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World");
                });
            });
        }
    }


# 2. DEPENDENCY INJECTION :
    Dependency Injection (DI) is a design pattern used to achieve Inversion of Control (IoC) between classes and their dependencies. It allows for decoupling of the construction of a class’s dependencies from its behavior, making the system more modular, easier to test, and more configurable.
        IoC : your code stops creating and controlling its dependencies, and instead some external framework creates them and gives them to you.
    
    public class OrderService
    {
        private readonly IEmailSender _emailSender;

        // Email sender is *injected* from outside
        public OrderService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void PlaceOrder()
        {
            // ... place order logic ...
            _emailSender.Send("Order placed");
        }
    }

in program.cs

// When something needs IEmailSender, create SmtpEmailSender
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

// When something needs OrderService, create OrderService
builder.Services.AddTransient<OrderService>();


# 3. HOW ASP.NET UTILISES DEPENDENCY INJECTION?

ASP.NET Core has a built-in DI container. You register services, and then ASP.NET Core injects (gives) them to you in controllers, middleware, and other components throughout the application, promoting loose coupling and testability.

# 4. How do you Register Services in ASP.NET Core?
Service registration is the process of adding classes to ASP.NET Core’s dependency injection container (using methods like AddSingleton, AddScoped, and AddTransient) so that the framework can create and supply those classes wherever they are needed in the application.

Transient: A new instance of the service is created each time it is requested from the service container.
Scoped: A new instance of the service is created once per request within the scope. It is the same within a request but different across different requests.
Singleton: A single instance of the service is created and shared throughout the application’s lifetime.

public class TransientService { public Guid Id { get; } = Guid.NewGuid(); }
public class ScopedService    { public Guid Id { get; } = Guid.NewGuid(); }
public class SingletonService { public Guid Id { get; } = Guid.NewGuid(); }

builder.Services.AddTransient<TransientService>();
builder.Services.AddScoped<ScopedService>();
builder.Services.AddSingleton<SingletonService>();


PROGRAM.CS VS STARTUP.CS : 

Old style (before .NET 6): Program.cs + Startup.cs
You had two files:
Program.cs
Entry point that sets up the host and uses Startup:

Startup.cs
This class had two important methods:
ConfigureServices = where you register services (AddTransient, AddScoped, etc.)
Configure = where you add middleware and endpoints.

# 5. HOW AUTHENTICATION IS HANDLED IN ASP.NET ?

In ASP.NET Core, authentication is handled by the built‑in authentication middleware and the DI container. You configure one or more authentication schemes (e.g., cookies, JWT bearer, external providers like Google) in Program.cs (or Startup in older versions) using AddAuthentication and the appropriate handlers (AddCookie, AddJwtBearer, etc.). Then you make sure app.UseAuthentication() and app.UseAuthorization() are in the middleware pipeline so each request is authenticated before authorization is checked. Finally, you protect endpoints using [Authorize] attributes or authorization policies, so only authenticated users (and optionally specific roles/claims) can access them.

# 6. Explain the differences between JWT, OAuth, and OpenID Connect.

JWT (JSON Web Token): A compact, URL-safe means of representing claims to be transferred between two parties. It’s a token format used in authentication and information exchange.
OAuth: An authorization framework that enables a third-party application to obtain limited access to an HTTP service. It’s about delegation of authorization.
OpenID Connect: A simple identity layer on top of OAuth 2.0, which allows clients to verify the identity of the end-user and to obtain basic profile information in an interoperable and REST-like manner


