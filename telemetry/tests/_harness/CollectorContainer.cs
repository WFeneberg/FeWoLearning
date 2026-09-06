using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A real OpenTelemetry Collector, for the 🐳 facts that want to prove the wire works.
///
/// It runs the plain distribution with one pipeline: an OTLP/HTTP receiver into the
/// <c>debug</c> exporter at detailed verbosity, which prints every span it accepts to
/// stdout. Asserting against those logs needs no second protocol, no query API and no
/// networking back into the test host - the container talks to nobody, it only listens.
/// </summary>
public sealed class CollectorContainer : IAsyncDisposable
{
    /// <summary>Pinned. A moving tag would make a green run today prove nothing tomorrow.</summary>
    private const string Image = "otel/opentelemetry-collector:0.117.0";

    private const int OtlpHttpPort = 4318;

    private const string Config = """
        receivers:
          otlp:
            protocols:
              http:
                endpoint: 0.0.0.0:4318
        exporters:
          debug:
            verbosity: detailed
        service:
          telemetry:
            logs:
              level: info
          pipelines:
            traces:
              receivers: [otlp]
              exporters: [debug]
        """;

    private readonly IContainer _container;

    private CollectorContainer(IContainer container) => _container = container;

    /// <summary>Where an OTLP/HTTP trace exporter should send spans.</summary>
    public Uri TracesEndpoint =>
        new($"http://{_container.Hostname}:{_container.GetMappedPublicPort(OtlpHttpPort)}/v1/traces");

    public static async Task<CollectorContainer> StartAsync()
    {
        var container = new ContainerBuilder(Image)
            .WithResourceMapping(Encoding.UTF8.GetBytes(Config), "/etc/otelcol/config.yaml")
            .WithPortBinding(OtlpHttpPort, assignRandomHostPort: true)
            .WithWaitStrategy(
                Wait.ForUnixContainer().UntilMessageIsLogged("Everything is ready"))
            .Build();

        await container.StartAsync();

        return new CollectorContainer(container);
    }

    /// <summary>
    /// Poll the container's output until <paramref name="needle"/> shows up, or give up.
    ///
    /// Polling rather than sleeping: the collector writes its debug output when it feels
    /// like it, and a fixed wait is either flaky or slow. Returns whatever was read last,
    /// so a failing assertion can show what actually arrived.
    /// </summary>
    public async Task<string> WaitForLogContaining(string needle, int timeoutSeconds = 30)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(timeoutSeconds);
        var seen = string.Empty;

        while (DateTimeOffset.UtcNow < deadline)
        {
            var (stdout, stderr) = await _container.GetLogsAsync();
            seen = stdout + stderr;

            if (seen.Contains(needle, StringComparison.Ordinal)) return seen;

            await Task.Delay(250);
        }

        return seen;
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
