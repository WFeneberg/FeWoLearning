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

    // A Button, not a TextBox: Button's default template resolves through
    // SystemResources without an Application, and its DesiredSize is 0x0 when that
    // resolution fails - which is what makes the smoke fact able to fail at all.
    public static System.Windows.Controls.Button MakeButton() => new() { Content = "smoke" };
}
