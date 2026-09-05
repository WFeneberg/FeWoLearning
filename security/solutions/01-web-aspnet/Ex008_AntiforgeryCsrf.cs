using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 008 — AntiforgeryCsrf (reference solution).
public static class Ex008_AntiforgeryCsrf
{
    // A fixed header name keeps validation independent of the request's content
    // type: with a form field name instead, ValidateRequestAsync would need to
    // read the request body as a form, consuming it before the downstream
    // handler ever sees it.
    private const string TokenHeaderName = "X-CSRF-TOKEN";

    public static void AddServices(IServiceCollection services) =>
        services.AddAntiforgery(options => options.HeaderName = TokenHeaderName);

    public static void Use(IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            var method = ctx.Request.Method;
            var isSafe = HttpMethods.IsGet(method) || HttpMethods.IsHead(method) ||
                         HttpMethods.IsOptions(method) || HttpMethods.IsTrace(method);

            if (!isSafe)
            {
                var antiforgery = ctx.RequestServices.GetRequiredService<IAntiforgery>();
                try
                {
                    await antiforgery.ValidateRequestAsync(ctx);
                }
                catch (AntiforgeryValidationException)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }

            await next();
        });
}
