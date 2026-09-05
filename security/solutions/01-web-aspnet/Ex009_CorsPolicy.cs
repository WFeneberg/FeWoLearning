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
                .WithOrigins(allowedOrigin)
                .WithMethods("GET", "POST")
                .AllowAnyHeader()));
    // No AllowCredentials(): the origin allowlist above is never "*", but the
    // combination this exercise warns against needs both halves absent by
    // construction, not merely untested.

    public static void Use(IApplicationBuilder app) =>
        app.UseCors(PolicyName);
}
