using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 019 — RateLimiting (reference solution).
public static class Ex019_RateLimiting
{
    private const string AnonymousPartition = "(no-api-key)";

    public static void AddServices(IServiceCollection services, int permitsPerWindow)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // A global limiter applies to every request through this pipeline
            // regardless of routing/endpoints, and PartitionedRateLimiter keys
            // each caller's own fixed window off X-Api-Key - so exhausting one
            // key's window never touches another key's budget.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var apiKey = httpContext.Request.Headers["X-Api-Key"].ToString();
                var partitionKey = string.IsNullOrEmpty(apiKey) ? AnonymousPartition : apiKey;

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitsPerWindow,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    // No automatic timer-driven replenishment: once a partition
                    // is exhausted it stays exhausted for the rest of the test,
                    // so nothing here depends on wall-clock timing.
                    AutoReplenishment = false,
                });
            });
        });
    }

    public static void Use(IApplicationBuilder app) => app.UseRateLimiter();
}
