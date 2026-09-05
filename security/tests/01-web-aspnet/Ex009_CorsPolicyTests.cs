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

    // Asserting only "not (wildcard AND credentials)" would be vacuous: a policy
    // that never allows credentials at all satisfies it without ever facing the
    // choice. So this fact first pins credentials ON - the response really does
    // carry Access-Control-Allow-Credentials: true - and only then demands that
    // the origin be the exact one rather than "*". That is the forbidden
    // combination, one edit away, with nowhere left to hide.
    [Fact]
    public async Task Attack_Credentials_Are_Allowed_Only_Alongside_An_Exact_Origin_Never_A_Wildcard()
    {
        await using var harness = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("Origin", AllowedOrigin);
        var response = await harness.Client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal("true", Assert.Single(response.Headers.GetValues("Access-Control-Allow-Credentials")));

        var origin = Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin"));
        Assert.NotEqual("*", origin);
        Assert.Equal(AllowedOrigin, origin);
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
