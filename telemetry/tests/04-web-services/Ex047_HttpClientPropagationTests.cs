using System.Diagnostics;
using System.Net;
using System.Net.Http;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex047_HttpClientPropagationTests
{
    /// <summary>
    /// Stands in for the network: it answers every request and keeps the headers that
    /// were on it, which is the only thing this row needs to see.
    /// </summary>
    private sealed class CapturingTransport : HttpMessageHandler
    {
        public Dictionary<string, string> LastHeaders { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastHeaders.Clear();
            foreach (var header in request.Headers) LastHeaders[header.Key] = string.Join(",", header.Value);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private static async Task<(Dictionary<string, string> Headers, List<Activity> Spans)> Call(
        bool insideAnAmbientSpan)
    {
        var exported = new List<Activity>();
        var transport = new CapturingTransport();

        using var provider = Ex047_HttpClientPropagation.Build(exported);

        var handler = Ex047_HttpClientPropagation.CreatePropagatingHandler();
        handler.InnerHandler = transport;
        using var client = new HttpClient(handler);

        if (insideAnAmbientSpan)
        {
            using var ambient = Ex047_HttpClientPropagation.Source.StartActivity("caller");
            await client.GetAsync("http://remote.invalid/remote");
        }
        else
        {
            await client.GetAsync("http://remote.invalid/remote");
        }

        provider.ForceFlush();

        return (transport.LastHeaders, exported);
    }

    [Fact]
    public async Task The_outgoing_request_carries_a_traceparent()
    {
        using var ctx = new TelemetryContext();

        var (headers, _) = await Call(insideAnAmbientSpan: true);

        var header = Assert.Contains(Ex047_HttpClientPropagation.TraceParentHeader, headers);
        Assert.Matches("^00-[0-9a-f]{32}-[0-9a-f]{16}-[0-9a-f]{2}$", header);
    }

    [Fact]
    public async Task The_client_span_is_a_child_of_whatever_was_ambient()
    {
        using var ctx = new TelemetryContext();

        var (_, spans) = await Call(insideAnAmbientSpan: true);

        var client = Assert.Single(spans, s => s.Kind == ActivityKind.Client);
        var caller = Assert.Single(spans, s => s.DisplayName == "caller");

        Assert.Equal(Ex047_HttpClientPropagation.ClientSpanName, client.DisplayName);
        Assert.Equal(caller.SpanId, client.ParentSpanId);
    }

    [Fact]
    public async Task Adversarial_A_The_header_names_the_client_span_not_the_ambient_one()
    {
        // Wrong in most hand-rolled propagation, and invisible in a two-service system.
        // Injecting the AMBIENT context sends the caller's own parent, so the remote span
        // attaches one level too high: the client span and the server span become siblings
        // instead of parent and child, the waterfall loses the network hop entirely, and
        // the time spent in transit is silently attributed to the caller.
        using var ctx = new TelemetryContext();

        var (headers, spans) = await Call(insideAnAmbientSpan: true);

        var client = Assert.Single(spans, s => s.Kind == ActivityKind.Client);
        var caller = Assert.Single(spans, s => s.DisplayName == "caller");

        var header = headers[Ex047_HttpClientPropagation.TraceParentHeader];
        Assert.Contains(client.SpanId.ToHexString(), header);
        Assert.DoesNotContain(caller.SpanId.ToHexString(), header);
    }

    [Fact]
    public async Task Adversarial_B_The_receiving_side_continues_the_same_trace()
    {
        // The whole point, stated end to end: one trace, three spans, and the network hop
        // visible as the gap between the client span and the server span.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();
        var transport = new CapturingTransport();

        using var provider = Ex047_HttpClientPropagation.Build(exported);

        var handler = Ex047_HttpClientPropagation.CreatePropagatingHandler();
        handler.InnerHandler = transport;
        using var client = new HttpClient(handler);

        using (Ex047_HttpClientPropagation.Source.StartActivity("caller"))
        {
            await client.GetAsync("http://remote.invalid/remote");
        }

        var server = Ex047_HttpClientPropagation.HandleIncoming(transport.LastHeaders);
        provider.ForceFlush();

        var clientSpan = Assert.Single(exported, s => s.Kind == ActivityKind.Client);
        Assert.NotNull(server);
        Assert.Equal(ActivityKind.Server, server.Kind);
        Assert.Equal(clientSpan.TraceId, server.TraceId);
        Assert.Equal(clientSpan.SpanId, server.ParentSpanId);
        Assert.True(server.HasRemoteParent);
    }

    [Fact]
    public async Task Adversarial_C_With_nothing_listening_the_request_still_goes_out()
    {
        // Row 015 arriving where it costs the most. StartActivity returns null with no
        // listener, so every line of a propagating handler has to work when there is no
        // span - the alternative is an application that throws in production and works in
        // every test, or one that quietly stops making outbound calls when telemetry is
        // switched off.
        using var ctx = new TelemetryContext();
        var transport = new CapturingTransport();

        var handler = Ex047_HttpClientPropagation.CreatePropagatingHandler();
        handler.InnerHandler = transport;
        using var client = new HttpClient(handler);

        // No provider anywhere: nothing is listening to the source at all.
        var response = await client.GetAsync("http://remote.invalid/remote");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
