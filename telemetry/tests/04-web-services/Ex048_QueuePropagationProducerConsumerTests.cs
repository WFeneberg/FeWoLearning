using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex048_QueuePropagationProducerConsumerTests
{
    private static (Activity Producer, Activity Consumer) Hop(bool keepHeaders = true)
    {
        var exported = new List<Activity>();

        using var provider = Ex048_QueuePropagationProducerConsumer.Build(exported);

        var message = Ex048_QueuePropagationProducerConsumer.Publish("order placed");
        if (!keepHeaders) message.Headers.Clear();

        Ex048_QueuePropagationProducerConsumer.Consume(message);
        provider.ForceFlush();

        return (
            Assert.Single(exported, s => s.Kind == ActivityKind.Producer),
            Assert.Single(exported, s => s.Kind == ActivityKind.Consumer));
    }

    [Fact]
    public void Publishing_produces_a_producer_span_and_a_message_carrying_the_context()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex048_QueuePropagationProducerConsumer.Build(exported);
        var message = Ex048_QueuePropagationProducerConsumer.Publish("order placed");
        provider.ForceFlush();

        var producer = Assert.Single(exported);
        Assert.Equal(ActivityKind.Producer, producer.Kind);
        Assert.Equal("order placed", message.Body);

        var header = Assert.Contains(
            Ex048_QueuePropagationProducerConsumer.TraceParentHeader, message.Headers);
        Assert.Contains(producer.SpanId.ToHexString(), header);
    }

    [Fact]
    public void Adversarial_A_The_trace_crosses_the_queue()
    {
        // HTTP propagation happens by itself once an instrumentation library is
        // registered. Queue propagation does not, and never will: a broker has no notion
        // of a header your tracing library recognises, no request/response pair to hook,
        // and often no connection at all between the two sides.
        //
        // Write it or the trace ends at the publish - which reads, on every dashboard, as
        // a system where nothing happens after an order is placed.
        using var ctx = new TelemetryContext();

        var (producer, consumer) = Hop();

        Assert.Equal(producer.TraceId, consumer.TraceId);
        Assert.Equal(producer.SpanId, consumer.ParentSpanId);
    }

    [Fact]
    public void Adversarial_B_The_consumers_parent_is_remote()
    {
        // What tells the SDK - and a ParentBased sampler - that this context arrived from
        // somewhere else rather than being opened in this process. A consumer that
        // reconstructs the context without marking it remote makes every message look
        // locally caused.
        using var ctx = new TelemetryContext();

        var (_, consumer) = Hop();

        Assert.True(consumer.HasRemoteParent);
    }

    [Fact]
    public void Adversarial_C_A_message_with_no_context_starts_a_fresh_trace()
    {
        // The case that arrives in production on day one: a message that was already in
        // the queue before you shipped the propagation, or one from a producer you do not
        // own.
        //
        // The correct answer is a new trace rather than an exception. An unlinked trace is
        // a gap in a picture; a throwing consumer is a poison message that blocks the
        // queue.
        using var ctx = new TelemetryContext();

        var (producer, consumer) = Hop(keepHeaders: false);

        Assert.NotEqual(producer.TraceId, consumer.TraceId);
        Assert.Equal(default, consumer.ParentSpanId);
    }

    [Fact]
    public void Both_spans_name_the_broker_and_the_queue()
    {
        using var ctx = new TelemetryContext();

        var (producer, consumer) = Hop();

        foreach (var span in new[] { producer, consumer })
        {
            Assert.Equal(
                Ex048_QueuePropagationProducerConsumer.MessagingSystem,
                span.GetTagItem(Ex048_QueuePropagationProducerConsumer.MessagingSystemAttribute)?.ToString());
            Assert.Equal(
                Ex048_QueuePropagationProducerConsumer.QueueName,
                span.GetTagItem(Ex048_QueuePropagationProducerConsumer.MessagingDestinationAttribute)?.ToString());
        }
    }
}
