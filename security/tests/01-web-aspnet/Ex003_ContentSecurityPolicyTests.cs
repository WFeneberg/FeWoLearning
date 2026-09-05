using System.Net.Http;
using System.Text.RegularExpressions;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex003_ContentSecurityPolicyTests
{
    private static Task<WebHarness> StartAsync(RequestDelegate terminal) =>
        WebHarness.StartAsync(
            services: null,
            configure: app =>
            {
                Ex003_ContentSecurityPolicy.Use(app);
                app.Run(terminal);
            },
            ct: TestContext.Current.CancellationToken);

    private static Task PlainBody(HttpContext ctx) => ctx.Response.WriteAsync("body");

    [Fact]
    public async Task Attack_The_Header_Locks_Down_Default_And_Object_Sources()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        var csp = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));
        Assert.Contains("default-src 'self'", csp);
        Assert.Contains("object-src 'none'", csp);
    }

    [Fact]
    public async Task Attack_The_Header_Never_Opens_Unsafe_Inline_Or_Unsafe_Eval()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        var csp = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));
        Assert.DoesNotContain("unsafe-inline", csp);
        Assert.DoesNotContain("unsafe-eval", csp);
    }

    [Fact]
    public async Task Attack_Two_Separate_Requests_Receive_Different_Nonces()
    {
        await using var harness = await StartAsync(PlainBody);

        var first = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var second = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        var firstNonce = ExtractNonce(Assert.Single(first.Headers.GetValues("Content-Security-Policy")));
        var secondNonce = ExtractNonce(Assert.Single(second.Headers.GetValues("Content-Security-Policy")));

        Assert.NotEqual(firstNonce, secondNonce);
    }

    [Fact]
    public async Task Use_GetNonce_Returns_The_Exact_Value_The_Headers_Script_Src_Carries()
    {
        string? captured = null;
        await using var harness = await StartAsync(ctx =>
        {
            captured = Ex003_ContentSecurityPolicy.GetNonce(ctx);
            return ctx.Response.WriteAsync("body");
        });

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        var csp = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));
        Assert.NotNull(captured);
        Assert.Equal(ExtractNonce(csp), captured);
    }

    [Fact]
    public async Task Use_The_Nonce_Decodes_To_At_Least_16_Bytes()
    {
        string? captured = null;
        await using var harness = await StartAsync(ctx =>
        {
            captured = Ex003_ContentSecurityPolicy.GetNonce(ctx);
            return ctx.Response.WriteAsync("body");
        });

        await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.NotNull(captured);
        var decoded = Convert.FromBase64String(captured!);
        Assert.True(decoded.Length >= 16, $"nonce decodes to only {decoded.Length} bytes");
    }

    private static string ExtractNonce(string csp)
    {
        var match = Regex.Match(csp, "'nonce-([^']+)'");
        Assert.True(match.Success, $"no nonce directive in '{csp}'");
        return match.Groups[1].Value;
    }
}
