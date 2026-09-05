using System.Net;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex001_SecurityHeadersTests
{
    private static Task<WebHarness> StartAsync(RequestDelegate terminal) =>
        WebHarness.StartAsync(
            services: null,
            configure: app =>
            {
                Ex001_SecurityHeaders.Use(app);
                app.Run(terminal);
            },
            ct: TestContext.Current.CancellationToken);

    private static Task PlainBody(HttpContext ctx) => ctx.Response.WriteAsync("body");

    [Theory]
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("X-Frame-Options", "DENY")]
    [InlineData("Referrer-Policy", "no-referrer")]
    public async Task Attack_Response_Always_Carries_The_Hardening_Header(string name, string expected)
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(expected, Assert.Single(response.Headers.GetValues(name)));
    }

    [Fact]
    public async Task Use_A_Deliberate_Downstream_Value_Is_Not_Clobbered()
    {
        await using var harness = await StartAsync(ctx =>
        {
            ctx.Response.Headers["Referrer-Policy"] = "same-origin";
            return ctx.Response.WriteAsync("body");
        });

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal("same-origin", Assert.Single(response.Headers.GetValues("Referrer-Policy")));
    }

    [Fact]
    public async Task Use_The_Response_Body_And_Status_Are_Untouched()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("body", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
