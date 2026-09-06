using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex058_MultiHopTraceCapstoneTests
{
    private static async Task<(List<Activity> Spans, Activity? Worker)> RunTheWholeFlow(
        bool keepMessageContext = true)
    {
        var exported = new List<Activity>();
        var queue = new List<QueueMessage>();

        Activity? worker;

        await using (var web = await WebProbe.StartAsync(
            services => Ex058_MultiHopTraceCapstone.ConfigureTracing(services, exported),
            endpoints => Ex058_MultiHopTraceCapstone.MapEndpoints(endpoints, queue)))
        {
            await web.Client.GetAsync(Ex058_MultiHopTraceCapstone.OrdersRoute);

            var message = Assert.Single(queue);
            if (!keepMessageContext) message.Headers.Clear();

            // The worker runs long after the request is over, in what would be a different
            // process, with no connection between them.
            worker = Ex058_MultiHopTraceCapstone.Process(message);

            web.Services.GetRequiredService<TracerProvider>().ForceFlush();
        }

        return (exported, worker);
    }

    [Fact]
    public async Task All_three_hops_are_recorded()
    {
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunTheWholeFlow();

        Assert.Contains(spans, s => s.Kind == ActivityKind.Server);
        Assert.Contains(spans, s => s.DisplayName == Ex058_MultiHopTraceCapstone.PublishSpanName);
        Assert.Contains(spans, s => s.DisplayName == Ex058_MultiHopTraceCapstone.ProcessSpanName);
    }

    [Fact]
    public async Task Adversarial_A_One_trace_spans_all_three()
    {
        // What this whole block has been building toward. A customer reports that an order
        // never shipped; you have the trace id from their request; the worker's span is on
        // it, an hour later, in a different process, with no connection between them.
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunTheWholeFlow();

        Assert.Equal(3, spans.Count);
        Assert.Single(spans.Select(s => s.TraceId).Distinct());
    }

    [Fact]
    public async Task Adversarial_B_The_chain_is_server_then_publish_then_process()
    {
        // Sharing a trace id is not enough: three spans could all hang off the server and
        // the queue hop would be invisible. The parentage is what makes the waterfall show
        // where the time went.
        using var ctx = new TelemetryContext();

        var (spans, _) = await RunTheWholeFlow();

        var server = Assert.Single(spans, s => s.Kind == ActivityKind.Server);
        var publish = Assert.Single(spans, s => s.DisplayName == Ex058_MultiHopTraceCapstone.PublishSpanName);
        var process = Assert.Single(spans, s => s.DisplayName == Ex058_MultiHopTraceCapstone.ProcessSpanName);

        Assert.Equal(server.SpanId, publish.ParentSpanId);
        Assert.Equal(publish.SpanId, process.ParentSpanId);
        Assert.True(process.HasRemoteParent);
    }

    [Fact]
    public async Task Adversarial_C_A_message_with_no_context_is_processed_on_a_root()
    {
        // Where this capstone says something the three rows it combines did not. Row 049
        // insisted a worker's iterations be roots; here the worker continues somebody
        // else's trace, and both are right.
        //
        // The unit of work has not changed - it is still one item. What changed is whether
        // a context arrived with it.
        using var ctx = new TelemetryContext();

        var (spans, worker) = await RunTheWholeFlow(keepMessageContext: false);

        Assert.NotNull(worker);
        Assert.Equal(default, worker.ParentSpanId);

        var server = Assert.Single(spans, s => s.Kind == ActivityKind.Server);
        Assert.NotEqual(server.TraceId, worker.TraceId);
    }

    [Fact]
    public async Task Adversarial_D_A_message_with_no_context_is_still_processed()
    {
        // The paired half. A worker that refuses an uninstrumented message is a worker
        // that stops working the first time somebody publishes from a service you do not
        // own - which is a far worse failure than a gap in a picture.
        using var ctx = new TelemetryContext();

        var (_, worker) = await RunTheWholeFlow(keepMessageContext: false);

        Assert.NotNull(worker);
        Assert.Equal(Ex058_MultiHopTraceCapstone.ProcessSpanName, worker.DisplayName);
        Assert.Equal(ActivityKind.Consumer, worker.Kind);
    }
}
