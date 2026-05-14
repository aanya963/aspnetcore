// mkdir LearningAspNetCore
// cd LearningAspNetCore
// dotnet new webapi -n MyLearningApi
// cd MyLearningApi
//dotnet add package Swashbuckle.AspNetCore

// builder.Services is where we register services (DI).
// app is where we configure middleware and map endpoints.
// app.Use... lines are middleware.
// app.MapControllers() maps attribute-routed controllers.
// using Microsoft.OpenApi.Models;
using Mylearningapi.Services;
using Mylearningapi.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>{
    options.LoginPath = "/login";
});

// AddAuthentication → Tells ASP.NET Core you want to use authentication.
// "MyCookieAuth" is the scheme name — basically a label for this authentication method.
// AddCookie → Configures cookie‑based authentication.
// This means when a user logs in, the server issues a cookie that identifies them.
// options.LoginPath = "/login"; → If someone tries to access a protected resource without being logged in, they’ll be redirected to /login.

builder.Services.AddAuthorization(options =>{
   options.AddPolicy("AdminOnly", policy =>
   {
       policy.RequireRole("Admin");
   });
});
// AddAuthorization → Adds rules for who can access what.
// Policy: "AdminOnly" → You define a named policy.
// policy.RequireRole("Admin") → This policy requires the user to have the role "Admin"

// Register our own service with DI
// AddTransient<OrderService>() means: every time something needs OrderService, create a new instance.

builder.Services.AddTransient<OrderService>();

builder.Services.AddTransient<TransientService>();
builder.Services.AddScoped<ScopedService>();
builder.Services.AddSingleton<SingletonService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//it is calling the extension one
app.UseRequestLogging();
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.MapGet("/login", ...)  :  Defines a new GET endpoint at /login. When you visit this URL, the code inside runs.
app.MapGet("/login", async (HttpContext context) =>
{
    // A claim is a piece of information about the user (like their name or role).
    // Here we’re hard‑coding a fake user called "testuser" with the role "Admin".

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.Role, "Admin") // user is Admin
    };

    // ClaimsIdentity = represents the user’s identity (who they are + their claims).
    // ClaimsPrincipal = wraps one or more identities. This is what ASP.NET Core uses to represent the logged‑in user.

    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync("MyCookieAuth", principal);
    // This issues an authentication cookie to the browser.
    // Next time the browser makes a request, it will send that cookie back, and ASP.NET Core will know the user is logged in as "testuser" with role "Admin"

    await context.Response.WriteAsync("You are logged in as Admin.");
});


app.Run();