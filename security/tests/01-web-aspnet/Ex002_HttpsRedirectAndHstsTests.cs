using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex002_HttpsRedirectAndHstsTests
{
    private const int HttpsPort = 5443;

    private static Task<WebHarness> StartAsync(RequestDelegate terminal) =>
        WebHarness.StartAsync(
            services: null,
            configure: app =>
            {
                Ex002_HttpsRedirectAndHsts.Use(app, HttpsPort);
                app.Run(terminal);
            },
            ct: TestContext.Current.CancellationToken);

    private static Task PlainBody(HttpContext ctx) => ctx.Response.WriteAsync("body");

    [Fact]
    public async Task Attack_A_Plain_Http_Request_Is_Redirected_To_Https_With_Path_And_Query_Preserved()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync(
            "http://localhost/reports/42?tab=summary", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.PermanentRedirect, response.StatusCode);
        var location = Assert.Single(response.Headers.GetValues("Location"));
        Assert.Equal($"https://localhost:{HttpsPort}/reports/42?tab=summary", location);
    }

    [Fact]
    public async Task Attack_An_Https_Response_Carries_Hsts_With_A_Long_MaxAge_And_IncludeSubDomains()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("https://localhost/", TestContext.Current.CancellationToken);

        var hsts = Assert.Single(response.Headers.GetValues("Strict-Transport-Security"));
        var match = Regex.Match(hsts, "max-age=(\\d+)");
        Assert.True(match.Success, $"no max-age directive in '{hsts}'");
        Assert.True(long.Parse(match.Groups[1].Value) >= 31536000, $"max-age too short in '{hsts}'");
        Assert.Contains("includeSubDomains", hsts, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Use_An_Https_Request_Is_Served_Directly_Not_Redirected()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("https://localhost/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("body", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Use_A_Plain_Http_Response_Never_Carries_Hsts()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("http://localhost/", TestContext.Current.CancellationToken);

        Assert.False(response.Headers.Contains("Strict-Transport-Security"));
    }
}
