using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 003 — ContentSecurityPolicy (reference solution).
public static class Ex003_ContentSecurityPolicy
{
    private const string NonceItemsKey = "Ex003.Nonce";

    public static void Use(IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            // A fresh nonce per request - a fixed one would let an attacker who ever
            // sees a single page reuse it on injected markup forever.
            var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));
            ctx.Items[NonceItemsKey] = nonce;

            ctx.Response.OnStarting(() =>
            {
                ctx.Response.Headers["Content-Security-Policy"] =
                    $"default-src 'self'; object-src 'none'; script-src 'self' 'nonce-{nonce}'";
                return Task.CompletedTask;
            });

            await next();
        });

    public static string GetNonce(HttpContext context) =>
        context.Items.TryGetValue(NonceItemsKey, out var value) && value is string nonce
            ? nonce
            : throw new InvalidOperationException(
                "Ex003_ContentSecurityPolicy.GetNonce was called on a request whose pipeline never ran Use");
}
