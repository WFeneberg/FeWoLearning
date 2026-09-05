using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 002 — HttpsRedirectAndHsts (reference solution).
public static class Ex002_HttpsRedirectAndHsts
{
    public static void Use(IApplicationBuilder app, int httpsPort) =>
        app.Use(async (ctx, next) =>
        {
            if (!ctx.Request.IsHttps)
            {
                // A request that never reached TLS earns a redirect, never HSTS - the
                // header would be an unkeepable promise on a channel already shown to be
                // downgradeable.
                var portSegment = httpsPort == 443 ? string.Empty : $":{httpsPort}";
                ctx.Response.StatusCode = StatusCodes.Status308PermanentRedirect;
                ctx.Response.Headers["Location"] =
                    $"https://{ctx.Request.Host.Host}{portSegment}{ctx.Request.Path}{ctx.Request.QueryString}";
                return;
            }

            ctx.Response.OnStarting(() =>
            {
                ctx.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
                return Task.CompletedTask;
            });

            await next();
        });
}
