using System.Net;
using System.Net.Http;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex009_CorsPolicyTests
{
    private const string AllowedOrigin = "https://good.example";

    private static Task<WebHarness> StartAsync() =>
        WebHarness.StartAsync(
            services: services => Ex009_CorsPolicy.AddServices(services, AllowedOrigin),
            configure: app =>
            {
                Ex009_CorsPolicy.Use(app);
                app.Run(ctx => ctx.Response.WriteAsync("body"));
            },
            ct: TestContext.Current.CancellationToken);

    [Fact]
    public async Task Attack_A_Disallowed_Origin_Receives_No_Cors_Header()
    {
        await using var harness = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("Origin", "https://evil.example");
        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Attack_The_Response_Never_Combines_Wildcard_Origin_With_Credentials()
    {
        await using var harness = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("Origin", AllowedOrigin);
        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        var isWildcardOrigin = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins) &&
                                origins.Contains("*");
        var allowsCredentials = response.Headers.TryGetValues("Access-Control-Allow-Credentials", out var creds) &&
                                 creds.Contains("true");

        Assert.False(isWildcardOrigin && allowsCredentials);
    }

    [Fact]
    public async Task Use_The_Allowed_Origin_Is_Echoed_Back()
    {
        await using var harness = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("Origin", AllowedOrigin);
        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(AllowedOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
    }

    [Fact]
    public async Task Use_A_Preflight_From_The_Allowed_Origin_Succeeds_With_The_Allowed_Methods()
    {
        await using var harness = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Options, "/");
        request.Headers.Add("Origin", AllowedOrigin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Contains("POST", response.Headers.GetValues("Access-Control-Allow-Methods").First());
    }
}
