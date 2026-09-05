using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.Support;

// Harness canary only. Never a catalog row, never a TODO: if these stop working,
// a package bump broke a harness, not an exercise.
public static class SmokeProbe
{
    public static void Configure(IApplicationBuilder app) =>
        app.Run(async ctx =>
        {
            ctx.Response.Headers["X-Smoke"] = "ok";
            await ctx.Response.WriteAsync("pong");
        });

    public static System.Windows.Controls.TextBox MakeTextBox() => new() { Text = "smoke" };
}
