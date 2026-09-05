using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 019 — RateLimiting (web-aspnet).
// Goal:   Cap requests per caller rather than per process: register a rate
//         limiter partitioned by the caller's X-Api-Key header, so exhausting
//         one key's allowance never touches a different key's budget, and
//         wire the middleware into the pipeline.
// Drills: rate limiter partitions, 429 responses, per-principal keys.
// Passes: attack fact    - the request past the permit count returns 429;
//         use facts      - every request up to the limit returns 200, and a
//                          different X-Api-Key still gets its own full
//                          allowance while the first is exhausted - the fact
//                          that proves partitioning rather than a global
//                          counter.
public static class Ex019_RateLimiting
{
    public static void AddServices(IServiceCollection services, int permitsPerWindow) =>
        throw new NotImplementedException(
            "TODO: Ex019 - register a rate limiter partitioned by the X-Api-Key header, permitsPerWindow permits per key, rejecting with 429 past the limit");

    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException("TODO: Ex019 - wire the rate limiter middleware into the pipeline");
}
