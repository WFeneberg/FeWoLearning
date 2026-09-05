using System.IO;
using System.Text.Json;
using FeWoLearning.Security.Exercises.WebAspNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex016_InsecureDirectObjectReferenceTests
{
    // IResult.ExecuteAsync resolves ILoggerFactory (and, for Results.Ok, the
    // JSON options) from HttpContext.RequestServices - an empty provider isn't
    // enough, AddLogging() is required or every ExecuteAsync throws.
    private static readonly IServiceProvider Services =
        new ServiceCollection().AddLogging().BuildServiceProvider();

    private static async Task<(int Status, byte[] Body)> Invoke(IResult result)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = Services,
        };
        var body = new MemoryStream();
        context.Response.Body = body;

        await result.ExecuteAsync(context);

        return (context.Response.StatusCode, body.ToArray());
    }

    [Fact]
    public async Task Attack_Someone_Elses_Invoice_And_A_Missing_Invoice_Are_Byte_Identical_404s()
    {
        var store = new[] { new Ex016_Invoice(1, "alice", 100m) };

        var (notMineStatus, notMineBody) = await Invoke(
            Ex016_InsecureDirectObjectReference.GetInvoice("mallory", 1, store));
        var (missingStatus, missingBody) = await Invoke(
            Ex016_InsecureDirectObjectReference.GetInvoice("mallory", 999, store));

        Assert.Equal(404, notMineStatus);
        Assert.Equal(404, missingStatus);
        Assert.Equal(missingBody, notMineBody);
    }

    [Fact]
    public async Task Use_The_Owner_Gets_200_With_The_Amount()
    {
        var store = new[] { new Ex016_Invoice(1, "alice", 100m) };

        var (status, body) = await Invoke(Ex016_InsecureDirectObjectReference.GetInvoice("alice", 1, store));

        Assert.Equal(200, status);
        Assert.Equal(100m, JsonSerializer.Deserialize<decimal>(body));
    }

    [Fact]
    public async Task Use_An_Owner_With_Two_Invoices_Can_Fetch_Both()
    {
        var store = new[] { new Ex016_Invoice(1, "alice", 100m), new Ex016_Invoice(2, "alice", 250m) };

        var (firstStatus, firstBody) = await Invoke(Ex016_InsecureDirectObjectReference.GetInvoice("alice", 1, store));
        var (secondStatus, secondBody) = await Invoke(Ex016_InsecureDirectObjectReference.GetInvoice("alice", 2, store));

        Assert.Equal(200, firstStatus);
        Assert.Equal(200, secondStatus);
        Assert.Equal(100m, JsonSerializer.Deserialize<decimal>(firstBody));
        Assert.Equal(250m, JsonSerializer.Deserialize<decimal>(secondBody));
    }
}
