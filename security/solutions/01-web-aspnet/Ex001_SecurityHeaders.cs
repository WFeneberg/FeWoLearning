using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 001 — SecurityHeaders (reference solution).
public static class Ex001_SecurityHeaders
{
    public static void Use(IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            // OnStarting, not a plain assignment before next(): the downstream handler
            // runs after this line, so anything set here can still be overwritten - and
            // anything set after next() is too late once the body has begun.
            ctx.Response.OnStarting(() =>
            {
                var headers = ctx.Response.Headers;
                if (!headers.ContainsKey("X-Content-Type-Options")) headers["X-Content-Type-Options"] = "nosniff";
                if (!headers.ContainsKey("X-Frame-Options")) headers["X-Frame-Options"] = "DENY";
                if (!headers.ContainsKey("Referrer-Policy")) headers["Referrer-Policy"] = "no-referrer";
                return Task.CompletedTask;
            });

            await next();
        });
}
