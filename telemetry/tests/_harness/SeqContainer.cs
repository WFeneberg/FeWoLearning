using System.Net.Http;
using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A real Seq instance, for the 🐳 fact that wants to prove a structured log is
/// genuinely queryable rather than merely well-shaped.
///
/// It is the third container shape in this harness and the most demanding: unlike
/// <see cref="CollectorContainer"/> it is asked a question afterwards, and unlike
/// <see cref="PromtoolContainer"/> it has to be reachable while the test runs.
///
/// Measured 2026-09-06: <c>ACCEPT_EULA=Y</c> is required, no authentication needs
/// configuring for anonymous ingestion and querying, <c>/ingest/clef</c> answers 201, and
/// <c>/api/events?filter=…</c> returns the event with its properties still typed - a
/// number arrives as a JSON number, and the message template comes back as tokens rather
/// than as rendered text.
/// </summary>
public sealed class SeqContainer : IAsyncDisposable
{
    /// <summary>Pinned. A moving tag would make a green run today prove nothing tomorrow.</summary>
    private const string Image = "datalust/seq:2024.3";

    private const int HttpPort = 80;

    private readonly IContainer _container;
    private readonly HttpClient _client;

    private SeqContainer(IContainer container)
    {
        _container = container;
        _client = new HttpClient
        {
            BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort(HttpPort)}"),
        };
    }

    public static async Task<SeqContainer> StartAsync()
    {
        var container = new ContainerBuilder(Image)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithPortBinding(HttpPort, assignRandomHostPort: true)
            .WithWaitStrategy(
                Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(
                    request => request.ForPort(HttpPort).ForPath("/api")))
            .Build();

        await container.StartAsync();

        return new SeqContainer(container);
    }

    /// <summary>POST newline-delimited CLEF; 201 means accepted.</summary>
    public async Task<System.Net.HttpStatusCode> IngestAsync(string clef)
    {
        using var content = new StringContent(clef, Encoding.UTF8);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            "application/vnd.serilog.clef");

        var response = await _client.PostAsync("/ingest/clef", content);

        return response.StatusCode;
    }

    /// <summary>
    /// Ask Seq a question in its own filter language and return the raw JSON answer.
    ///
    /// Polled rather than read once: ingestion is asynchronous, so the first query after
    /// a POST can legitimately come back empty.
    /// </summary>
    public async Task<string> QueryAsync(string filter, int timeoutSeconds = 30)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(timeoutSeconds);
        var seen = "[]";

        while (DateTimeOffset.UtcNow < deadline)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"/api/events?filter={Uri.EscapeDataString(filter)}&count=10");
            request.Headers.Add("Accept", "application/json");

            var response = await _client.SendAsync(request);
            seen = await response.Content.ReadAsStringAsync();

            if (seen.Length > 2) return seen;

            await Task.Delay(250);
        }

        return seen;
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _container.DisposeAsync();
    }
}
