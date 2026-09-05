using System.Net;
using System.Net.Http;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex019_RateLimitingTests
{
    private static Task<WebHarness> StartAsync(int permitsPerWindow) =>
        WebHarness.StartAsync(
            services: services => Ex019_RateLimiting.AddServices(services, permitsPerWindow),
            configure: app =>
            {
                Ex019_RateLimiting.Use(app);
                app.Run(ctx => ctx.Response.WriteAsync("ok"));
            },
            ct: TestContext.Current.CancellationToken);

    private static HttpRequestMessage Request(string apiKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Api-Key", apiKey);
        return request;
    }

    [Fact]
    public async Task Use_Every_Request_Up_To_The_Limit_Returns_200()
    {
        await using var harness = await StartAsync(permitsPerWindow: 3);

        for (var i = 0; i < 3; i++)
        {
            var response = await harness.Client.SendAsync(Request("key-a"), TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task Attack_The_Request_Past_The_Permit_Count_Returns_429()
    {
        await using var harness = await StartAsync(permitsPerWindow: 3);

        for (var i = 0; i < 3; i++)
        {
            await harness.Client.SendAsync(Request("key-a"), TestContext.Current.CancellationToken);
        }

        var response = await harness.Client.SendAsync(Request("key-a"), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    [Fact]
    public async Task Use_A_Different_Api_Key_Still_Gets_Its_Own_Full_Allowance()
    {
        await using var harness = await StartAsync(permitsPerWindow: 3);

        for (var i = 0; i < 3; i++)
        {
            var exhausting = await harness.Client.SendAsync(Request("key-a"), TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, exhausting.StatusCode);
        }

        var rejected = await harness.Client.SendAsync(Request("key-a"), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);

        var otherKeyResponse = await harness.Client.SendAsync(Request("key-b"), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, otherKeyResponse.StatusCode);
    }
}
