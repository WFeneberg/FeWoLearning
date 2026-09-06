using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// An in-memory ASP.NET Core host, for the rows whose subject is what the framework
/// records about a request.
///
/// It is a real pipeline - routing, endpoints, the whole middleware chain - over an
/// in-memory transport, so `AddAspNetCoreInstrumentation` behaves exactly as it does in
/// production. Measured 2026-09-06: a request to a mapped endpoint produces one Server
/// span already named after the ROUTE TEMPLATE.
///
/// What it cannot do, also measured: exercise `AddHttpClientInstrumentation`. The
/// diagnostics handler that instrumentation listens to is inserted by the real socket
/// handler chain, and a client built over any custom handler - which is what
/// <c>TestServer</c> hands you - never goes through it, so an outgoing call produces
/// zero spans. That is a property of the transport, not of the instrumentation, and it
/// is why row 041 is built on the server side.
/// </summary>
public sealed class WebProbe : IAsyncDisposable
{
    private readonly IHost _host;

    private WebProbe(IHost host)
    {
        _host = host;
        Client = host.GetTestClient();
    }

    /// <summary>An HttpClient wired straight into the in-memory server.</summary>
    public HttpClient Client { get; }

    /// <summary>The host's services, for resolving a provider to flush.</summary>
    public IServiceProvider Services => _host.Services;

    /// <summary>
    /// Start a host whose services are configured by <paramref name="configureServices"/>
    /// and whose endpoints are mapped by <paramref name="mapEndpoints"/>.
    /// </summary>
    /// <param name="configureMiddleware">
    /// Runs before <c>UseRouting</c>, for the rows whose subject is middleware rather
    /// than an endpoint - the Prometheus scraping endpoint, for one.
    /// </param>
    public static async Task<WebProbe> StartAsync(
        Action<IServiceCollection> configureServices,
        Action<IEndpointRouteBuilder> mapEndpoints,
        Action<IApplicationBuilder>? configureMiddleware = null)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(services =>
                {
                    services.AddRouting();
                    configureServices(services);
                });
                web.Configure(app =>
                {
                    configureMiddleware?.Invoke(app);
                    app.UseRouting();
                    app.UseEndpoints(mapEndpoints);
                });
            })
            .StartAsync();

        return new WebProbe(host);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
