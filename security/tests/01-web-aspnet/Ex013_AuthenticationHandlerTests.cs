using System.Net;
using System.Net.Http;
using System.Security.Claims;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex013_AuthenticationHandlerTests
{
    private const string ValidKey = "s3cr3t-api-key";

    private static Task<WebHarness> StartAsync() =>
        WebHarness.StartAsync(
            services: s => Ex013_AuthenticationHandler.AddServices(s, ValidKey),
            configure: app =>
            {
                app.UseAuthentication();
                app.Run(async ctx =>
                {
                    var result = await ctx.AuthenticateAsync(Ex013_AuthenticationHandler.SchemeName);
                    if (!result.Succeeded)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    var isAuthenticated = result.Principal?.Identity?.IsAuthenticated ?? false;
                    var nameIdentifier = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    ctx.Response.Headers["X-Authenticated"] = isAuthenticated.ToString();
                    ctx.Response.Headers["X-Name-Identifier"] = nameIdentifier ?? "";
                    await ctx.Response.WriteAsync("ok");
                });
            },
            ct: TestContext.Current.CancellationToken);

    private static HttpRequestMessage Request(string? apiKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        if (apiKey is not null)
        {
            request.Headers.Add("X-Api-Key", apiKey);
        }

        return request;
    }

    [Fact]
    public async Task Attack_No_Api_Key_Header_Fails_Authentication()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.SendAsync(Request(null), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Attack_A_Wrong_Api_Key_Fails_Authentication()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.SendAsync(Request("wrong-key"), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Attack_A_Key_Differing_Only_In_Case_Fails_Authentication()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.SendAsync(
            Request(ValidKey.ToUpperInvariant()), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Use_The_Valid_Key_Reaches_The_Endpoint_With_An_Authenticated_Principal()
    {
        await using var harness = await StartAsync();

        var response = await harness.Client.SendAsync(Request(ValidKey), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("True", response.Headers.GetValues("X-Authenticated").Single());
        Assert.False(string.IsNullOrEmpty(response.Headers.GetValues("X-Name-Identifier").Single()));
    }
}
