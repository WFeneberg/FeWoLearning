using System.Net;
using System.Net.Http;
using System.Text.Json;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex024_ErrorHandlingWithoutLeakageTests
{
    private const string LeakyMessage = "Connection failed: Server=db.internal;User Id=sa;Password=Sup3rSecret!;";

    private static Task<WebHarness> StartAsync(RequestDelegate terminal) =>
        WebHarness.StartAsync(
            services: null,
            configure: app =>
            {
                Ex024_ErrorHandlingWithoutLeakage.Use(app);
                app.Run(terminal);
            },
            ct: TestContext.Current.CancellationToken);

    private static Task Throws(HttpContext ctx) => throw new InvalidOperationException(LeakyMessage);

    [Fact]
    public async Task Attack_The_Body_Never_Contains_The_Original_Exception_Message()
    {
        await using var harness = await StartAsync(Throws);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.DoesNotContain("Sup3rSecret", body);
        Assert.DoesNotContain("db.internal", body);
    }

    [Fact]
    public async Task Attack_The_Body_Never_Contains_The_Exception_Type_Name()
    {
        await using var harness = await StartAsync(Throws);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.DoesNotContain(nameof(InvalidOperationException), body);
    }

    [Fact]
    public async Task Attack_The_Body_Never_Contains_A_Stack_Trace()
    {
        await using var harness = await StartAsync(Throws);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.DoesNotContain("at FeWoLearning", body);
    }

    [Fact]
    public async Task Use_The_Response_Is_A_Stable_ProblemDetails_500()
    {
        await using var harness = await StartAsync(Throws);

        var first = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);
        var second = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, first.StatusCode);
        Assert.Equal("application/problem+json", first.Content.Headers.ContentType?.MediaType);

        var firstBody = await first.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var secondBody = await second.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        using var firstDoc = JsonDocument.Parse(firstBody);
        using var secondDoc = JsonDocument.Parse(secondBody);

        Assert.Equal(500, firstDoc.RootElement.GetProperty("status").GetInt32());
        var title = firstDoc.RootElement.GetProperty("title").GetString();
        Assert.False(string.IsNullOrWhiteSpace(title));
        Assert.Equal(title, secondDoc.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task Use_A_Request_That_Does_Not_Throw_Passes_Through_Untouched()
    {
        await using var harness = await StartAsync(ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status201Created;
            return ctx.Response.WriteAsync("created");
        });

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("created", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
