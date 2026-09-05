using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 009 — CorsPolicy (reference solution).
public static class Ex009_CorsPolicy
{
    private const string PolicyName = "Ex009AllowedOrigin";

    public static void AddServices(IServiceCollection services, string allowedOrigin) =>
        services.AddCors(options =>
            options.AddPolicy(PolicyName, policy => policy
                // An explicit origin, never AllowAnyOrigin(). AllowCredentials()
                // is what makes that choice load-bearing rather than merely
                // tidy: the CORS protocol forbids "*" alongside credentials, and
                // ASP.NET Core enforces it - CorsPolicyBuilder.Build() throws
                // InvalidOperationException for AllowAnyOrigin().AllowCredentials(),
                // so the wildcard shortcut is not available here at all.
                .WithOrigins(allowedOrigin)
                .WithMethods("GET", "POST")
                .AllowAnyHeader()
                .AllowCredentials()));

    public static void Use(IApplicationBuilder app) =>
        app.UseCors(PolicyName);
}
