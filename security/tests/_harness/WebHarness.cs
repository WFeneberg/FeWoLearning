using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Security.Tests.Harness;

// Hosts an exercise's pipeline in an in-memory TestServer and hands back a client.
//
// UseTestServer lives here and ONLY here. The content libraries must never
// reference Microsoft.AspNetCore.TestHost: the whole point of the block-01 shape
// is that the learner configures a pipeline and the harness drives it. An
// exercise that could host itself would let a solution pass by bypassing the
// pipeline entirely.
public sealed class WebHarness : IAsyncDisposable
{
    private readonly IHost _host;

    private WebHarness(IHost host)
    {
        _host = host;
        Client = host.GetTestClient();
    }

    public HttpClient Client { get; }

    public IServiceProvider Services => _host.Services;

    public static async Task<WebHarness> StartAsync(
        Action<IServiceCollection>? services,
        Action<IApplicationBuilder> configure,
        CancellationToken ct = default)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(s => services?.Invoke(s));
                web.Configure(configure);
            })
            .StartAsync(ct);

        return new WebHarness(host);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
