using System.IO;
using System.Net;
using System.Net.Http;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex008_AntiforgeryCsrfTests
{
    private static Task<WebHarness> StartAsync(Func<HttpContext, Task>? onProtected = null) =>
        WebHarness.StartAsync(
            services: Ex008_AntiforgeryCsrf.AddServices,
            configure: app =>
            {
                Ex008_AntiforgeryCsrf.Use(app);
                app.Run(async ctx =>
                {
                    if (ctx.Request.Path == "/token")
                    {
                        var antiforgery = ctx.RequestServices.GetRequiredService<IAntiforgery>();
                        var tokens = antiforgery.GetAndStoreTokens(ctx);
                        await ctx.Response.WriteAsync(tokens.RequestToken ?? "");
                        return;
                    }

                    if (onProtected is not null)
                    {
                        await onProtected(ctx);
                    }
                    else
                    {
                        await ctx.Response.WriteAsync("ok");
                    }
                });
            },
            ct: TestContext.Current.CancellationToken);

    private static async Task<(string Token, string Cookie)> IssueTokenAsync(WebHarness harness)
    {
        var response = await harness.Client.GetAsync("/token", TestContext.Current.CancellationToken);
        var token = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var cookie = response.Headers.GetValues("Set-Cookie").First().Split(';')[0];
        return (token, cookie);
    }

    [Fact]
    public async Task Attack_A_Post_With_No_Token_Is_Rejected()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.PostAsync(
            "/", new StringContent("payload"), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attack_A_Post_With_A_Token_But_A_Mismatched_Cookie_Is_Rejected()
    {
        await using var harness = await StartAsync();
        var (token, cookie) = await IssueTokenAsync(harness);
        var cookieName = cookie.Split('=')[0];

        using var request = new HttpRequestMessage(HttpMethod.Post, "/") { Content = new StringContent("payload") };
        request.Headers.Add("X-CSRF-TOKEN", token);
        request.Headers.Add("Cookie", $"{cookieName}=tampered-value");

        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Use_A_Get_Is_Never_Challenged()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.GetAsync("/token", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Use_A_Post_With_A_Matching_Token_And_Cookie_Succeeds_And_The_Body_Is_Observed()
    {
        string? capturedBody = null;
        await using var harness = await StartAsync(async ctx =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            capturedBody = await reader.ReadToEndAsync(ctx.RequestAborted);
            await ctx.Response.WriteAsync("ok");
        });
        var (token, cookie) = await IssueTokenAsync(harness);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/") { Content = new StringContent("payload") };
        request.Headers.Add("X-CSRF-TOKEN", token);
        request.Headers.Add("Cookie", cookie);

        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("payload", capturedBody);
    }
}
