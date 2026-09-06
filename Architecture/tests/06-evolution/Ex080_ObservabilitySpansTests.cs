using FeWoLearning.Architecture.Exercises.Evolution.Ex080;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex080_ObservabilitySpansTests
{
    /// <summary>
    /// Predictable ids, so a fact can name them - and deliberately WITHOUT a hyphen.
    /// The wire format is "traceId-spanId-flags", so an id containing the delimiter makes
    /// every header unparseable and every propagation fact silently fall back to a new
    /// root. Measured: the first draft used "id-01" and three facts failed against a
    /// correct solution. Real trace ids are hex for the same reason.
    /// </summary>
    private static Func<string> Ids()
    {
        var next = 0;
        return () => $"id{++next:D2}";
    }

    private static Tracer Tracer(double rolls, double rate = 1.0) => new(() => rolls, rate, Ids());

    [Fact]
    public void A_Root_Span_Starts_A_Trace()
    {
        var root = Tracer(0.0).StartRoot("GET /orders");

        Assert.Null(root.ParentSpanId);
        Assert.NotEqual(root.TraceId, root.SpanId);
        Assert.Equal("GET /orders", root.Name);
    }

    [Fact]
    public void A_Child_Shares_The_Trace_And_Points_At_Its_Parent()
    {
        var tracer = Tracer(0.0);
        var root = tracer.StartRoot("GET /orders");

        var child = tracer.StartChild(root, "SELECT orders");

        Assert.Equal(root.TraceId, child.TraceId);
        Assert.Equal(root.SpanId, child.ParentSpanId);
        Assert.NotEqual(root.SpanId, child.SpanId);
    }

    [Fact]
    public void Mechanism_The_Sampling_Decision_Is_Made_Once_And_Inherited()
    {
        // Deciding per span produces traces with holes - half the spans of one request
        // kept, half dropped - which is strictly worse than dropping the request entirely,
        // because the gap looks like the work never happened rather than like a trace
        // nobody kept. The random source here would say "no" for every child.
        var rolls = new Queue<double>([0.0, 0.9, 0.9, 0.9]);
        var tracer = new Tracer(rolls.Dequeue, sampleRate: 0.5, Ids());

        var root = tracer.StartRoot("GET /orders");
        var child = tracer.StartChild(root, "SELECT orders");
        var grandchild = tracer.StartChild(child, "cache lookup");

        Assert.True(root.Sampled);
        Assert.True(child.Sampled);
        Assert.True(grandchild.Sampled);
    }

    [Fact]
    public void The_Sample_Rate_Is_Honoured_At_The_Root()
    {
        Assert.True(Tracer(0.05, rate: 0.1).StartRoot("GET /orders").Sampled);
        Assert.False(Tracer(0.5, rate: 0.1).StartRoot("GET /orders").Sampled);
    }

    [Fact]
    public void Mechanism_The_Decision_Survives_A_Process_Boundary()
    {
        // Head-based sampling is the reason the flag travels in the header at all. The
        // downstream tracer must not roll again - its own random source here would say no.
        var upstream = Tracer(0.0, rate: 1.0);
        var root = upstream.StartRoot("GET /orders");
        var header = upstream.Serialize(root);

        var downstream = Tracer(0.99, rate: 0.01);
        var remote = downstream.Continue(header, "POST /payments");

        Assert.True(remote.Sampled);
        Assert.Equal(root.TraceId, remote.TraceId);
        Assert.Equal(root.SpanId, remote.ParentSpanId);
    }

    [Fact]
    public void An_Unsampled_Trace_Stays_Unsampled_Downstream()
    {
        // The pair of the fact above: inheritance must carry "no" as faithfully as "yes",
        // or every downstream service quietly re-enables full sampling and the bill
        // arrives at the end of the month.
        var upstream = Tracer(0.99, rate: 0.01);
        var header = upstream.Serialize(upstream.StartRoot("GET /orders"));

        var downstream = Tracer(0.0, rate: 1.0);

        Assert.False(downstream.Continue(header, "POST /payments").Sampled);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("garbage")]
    [InlineData("only-two")]
    [InlineData("trace-span-XX")]
    [InlineData("-span-01")]
    public void Mechanism_A_Malformed_Header_Starts_A_New_Trace_Rather_Than_Throwing(string? header)
    {
        // A bad header from somebody else's client must never be able to fail the request.
        // The worst it may cost is one broken trace - and starting a fresh one at least
        // keeps this process's own work visible.
        var tracer = Tracer(0.0, rate: 1.0);

        var span = tracer.Continue(header, "POST /payments");

        Assert.Null(span.ParentSpanId);
        Assert.NotEmpty(span.TraceId);
        Assert.Equal("POST /payments", span.Name);
    }

    [Fact]
    public void Serialize_Round_Trips_Through_Continue()
    {
        var tracer = Tracer(0.0, rate: 1.0);
        var root = tracer.StartRoot("GET /orders");

        var remote = tracer.Continue(tracer.Serialize(root), "POST /payments");

        Assert.Equal(root.TraceId, remote.TraceId);
        Assert.Equal(root.SpanId, remote.ParentSpanId);
        Assert.Equal(root.Sampled, remote.Sampled);
    }
}
