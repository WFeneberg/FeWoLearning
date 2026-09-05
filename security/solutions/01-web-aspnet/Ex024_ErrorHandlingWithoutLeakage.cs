using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 024 — ErrorHandlingWithoutLeakage (reference solution).
public static class Ex024_ErrorHandlingWithoutLeakage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static void Use(IApplicationBuilder app) =>
        // UseExceptionHandler only ever runs its branch when something *downstream*
        // throws - a request that completes normally never reaches this lambda at
        // all, which is exactly what keeps this from becoming "answer 500 for
        // everything".
        app.UseExceptionHandler(errorApp => errorApp.Run(async ctx =>
        {
            // Deliberately built from nothing but a fixed string and the status
            // code: no IExceptionHandlerPathFeature is read here, so the caught
            // exception's message, type and stack never have a path into the
            // response at all.
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
            };

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions), ctx.RequestAborted);
        }));
}
