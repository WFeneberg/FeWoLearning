using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Architecture.Exercises.Web;

// Exercise 004 — MiddlewarePipeline (reference solution).
public static class Ex004_MiddlewarePipeline
{
    public const string ShortCircuitHeader = "X-Stop-Here";

    public static RequestDelegate Build(IServiceProvider services, IList<string> log)
    {
        var app = new ApplicationBuilder(services);

        app.Use(async (context, next) =>
        {
            log.Add("outer:in");
            await next(context);
            // Reached on EVERY path, including the short-circuited one: `next` returned
            // normally, it just never went any deeper.
            log.Add("outer:out");
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.ContainsKey(ShortCircuitHeader))
            {
                log.Add("gate:short-circuit");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return; // not calling next is what "short-circuit" means
            }

            log.Add("gate:in");
            await next(context);
            log.Add("gate:out");
        });

        app.Run(context =>
        {
            log.Add("terminal");
            context.Response.StatusCode = StatusCodes.Status202Accepted;
            return Task.CompletedTask;
        });

        return app.Build();
    }
}
