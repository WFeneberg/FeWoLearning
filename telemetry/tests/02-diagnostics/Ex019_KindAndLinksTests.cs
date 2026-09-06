using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex019_KindAndLinksTests
{
    private static TraceProbe Probe() => new(Ex019_KindAndLinks.SourceName);

    private static List<ActivityContext> PublishThree() =>
    [
        Ex019_KindAndLinks.Enqueue("a"),
        Ex019_KindAndLinks.Enqueue("b"),
        Ex019_KindAndLinks.Enqueue("c"),
    ];

    [Fact]
    public void Publishing_produces_a_producer_activity_and_hands_back_its_own_context()
    {
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        var context = Ex019_KindAndLinks.Enqueue("a");

        var published = probe.Single();
        Assert.Equal(ActivityKind.Producer, published.Kind);
        Assert.Equal("a", published.GetTagItem(Ex019_KindAndLinks.MessageTag)?.ToString());

        // Its OWN context - the thing a real producer writes into the message headers.
        // Returning the parent's, or a fresh one, loses the hop.
        Assert.Equal(published.SpanId, context.SpanId);
        Assert.Equal(published.TraceId, context.TraceId);
    }

    [Fact]
    public void Consuming_produces_one_consumer_activity_for_the_whole_batch()
    {
        // Kind is not decoration: a backend uses Producer and Consumer to recognise a
        // queue hop and stop counting the queue's own latency as the service's. Get it
        // wrong and the shape of every latency chart is wrong.
        using var ctx = new TelemetryContext();
        using var probe = Probe();
        var incoming = PublishThree();

        var batch = Ex019_KindAndLinks.ProcessBatch(incoming);

        Assert.NotNull(batch);
        Assert.Equal(ActivityKind.Consumer, batch.Kind);
        Assert.Equal("3", batch.GetTagItem(Ex019_KindAndLinks.BatchSizeTag)?.ToString());
    }

    [Fact]
    public void Adversarial_A_The_batch_activity_is_a_root_with_no_parent()
    {
        // The entire point, and the thing every first attempt gets wrong. Parenthood is
        // singular: a span has one parent because one thing caused it. A batch consumer
        // is caused by twenty messages from twenty unrelated traces. Picking one as the
        // parent is not a simplification, it is a false statement - it grafts the batch
        // onto one customer's trace, and the other nineteen end at the queue with no
        // visible continuation.
        // Run under an AMBIENT span deliberately. Measured 2026-09-06 and originally got
        // wrong here: `parentContext: default` does NOT make a root when something is
        // ambient - the activity inherits Activity.Current anyway - and neither does
        // `parentId: null`. On a bare thread both look correct, so a test that does not
        // open an ambient span cannot tell a real root from an accidental one.
        using var ctx = new TelemetryContext();
        using var probe = Probe();
        var incoming = PublishThree();

        Activity? batch;
        using (var ambient = Ex019_KindAndLinks.Source.StartActivity("consumer-loop"))
        {
            Assert.NotNull(ambient);
            batch = Ex019_KindAndLinks.ProcessBatch(incoming);

            // And the caller's own context is handed back untouched.
            Assert.Same(ambient, Activity.Current);
        }

        Assert.NotNull(batch);
        Assert.Equal(default, batch.ParentSpanId);
        Assert.All(incoming, c => Assert.NotEqual(c.TraceId, batch.TraceId));
    }

    [Fact]
    public void Adversarial_B_Every_message_is_linked_in_order()
    {
        // "Related to, not caused by". Twenty links point back at twenty producers, so
        // every one of those traces can find where its message went without any of them
        // owning the batch. Dropping or reordering them loses exactly that.
        using var ctx = new TelemetryContext();
        using var probe = Probe();
        var incoming = PublishThree();

        var batch = Ex019_KindAndLinks.ProcessBatch(incoming);

        Assert.NotNull(batch);
        var links = batch.Links.ToArray();
        Assert.Equal(3, links.Length);
        Assert.Equal(
            incoming.Select(c => c.SpanId),
            links.Select(l => l.Context.SpanId));
        Assert.Equal(
            incoming.Select(c => c.TraceId),
            links.Select(l => l.Context.TraceId));
    }

    [Fact]
    public void Each_published_message_gets_its_own_trace()
    {
        // Not decoration either: it is what makes Adversarial_A meaningful. If all
        // three publishes shared one trace, "the batch is not on any of their traces"
        // would be a much weaker statement.
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        var incoming = PublishThree();

        Assert.Equal(3, incoming.Select(c => c.TraceId).Distinct().Count());
    }
}
