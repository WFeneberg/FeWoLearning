using System;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex100_BackpressurePipelineTests
{
    [Fact]
    public void RejectsNonPositiveCapacity()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new BackpressurePipeline<int>(0));

    [Fact]
    public async Task ProducerSuspendsWhenChannelIsFull_AndInFlightNeverExceedsCapacity()
    {
        var pipeline = new BackpressurePipeline<int>(capacity: 2);

        // First two writes have free capacity: they must complete synchronously,
        // i.e. without ever suspending the "producer".
        var write1 = pipeline.ProduceAsync(1);
        Assert.True(write1.IsCompletedSuccessfully);
        var write2 = pipeline.ProduceAsync(2);
        Assert.True(write2.IsCompletedSuccessfully);

        Assert.Equal(2, pipeline.ProducedCount);
        Assert.Equal(2, pipeline.InFlightCount);
        Assert.Equal(2, pipeline.MaxObservedInFlight);

        // The channel is now at capacity: a third write must NOT complete
        // synchronously — this is the backpressure throttling the producer.
        var write3 = pipeline.ProduceAsync(3);
        Assert.False(write3.IsCompleted);
        Assert.Equal(2, pipeline.ProducedCount); // still only 2 — the write is suspended.

        // In-flight can never exceed the configured bound, no matter how many
        // more items are queued up behind the blocked write.
        Assert.True(pipeline.InFlightCount <= 2);
        Assert.True(pipeline.MaxObservedInFlight <= 2);

        await using var enumerator = pipeline.ConsumeAsync().GetAsyncEnumerator();

        // Draining exactly one item frees a slot, which must unblock write3.
        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(1, enumerator.Current);

        await write3; // must now complete promptly — no deadlock, no polling needed.
        Assert.Equal(3, pipeline.ProducedCount);
        Assert.Equal(1, pipeline.ConsumedCount);
        Assert.Equal(2, pipeline.InFlightCount); // items 2 and 3 now sit in the channel.
        Assert.Equal(2, pipeline.MaxObservedInFlight); // never exceeded the bound.

        // Drain item 2: the channel (items 2,3) still has room for one more only
        // once a second slot is freed.
        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(2, enumerator.Current);
        Assert.Equal(2, pipeline.ConsumedCount);
        Assert.Equal(1, pipeline.InFlightCount); // only item 3 remains queued.

        // One free slot: write4 completes synchronously, filling the channel
        // again (items 3, 4) — write5 must then suspend.
        var write4 = pipeline.ProduceAsync(4);
        Assert.True(write4.IsCompletedSuccessfully);
        var write5 = pipeline.ProduceAsync(5);
        Assert.False(write5.IsCompleted);
        Assert.Equal(4, pipeline.ProducedCount);
        Assert.Equal(2, pipeline.MaxObservedInFlight);

        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(3, enumerator.Current);
        await write5; // consuming item 3 frees the slot write5 was waiting for.
        Assert.Equal(5, pipeline.ProducedCount);

        // No more writes will ever arrive — safe to complete now that every
        // ProduceAsync call has already resolved.
        pipeline.Complete();

        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(4, enumerator.Current);
        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(5, enumerator.Current);

        // Channel was completed and fully drained: enumeration ends.
        Assert.False(await enumerator.MoveNextAsync());

        Assert.Equal(5, pipeline.ProducedCount);
        Assert.Equal(5, pipeline.ConsumedCount);
        Assert.Equal(0, pipeline.InFlightCount);
        Assert.Equal(2, pipeline.MaxObservedInFlight);
    }

    [Fact]
    public async Task CompleteWithoutItems_DrainsToEmptySequence()
    {
        var pipeline = new BackpressurePipeline<string>(capacity: 4);
        pipeline.Complete();

        await using var enumerator = pipeline.ConsumeAsync().GetAsyncEnumerator();
        Assert.False(await enumerator.MoveNextAsync());
        Assert.Equal(0, pipeline.ProducedCount);
        Assert.Equal(0, pipeline.ConsumedCount);
        Assert.Equal(0, pipeline.MaxObservedInFlight);
    }

    [Fact]
    public async Task CompleteWithError_PropagatesToConsumer()
    {
        var pipeline = new BackpressurePipeline<int>(capacity: 1);
        await pipeline.ProduceAsync(42);
        pipeline.Complete(new InvalidOperationException("upstream failed"));

        await using var enumerator = pipeline.ConsumeAsync().GetAsyncEnumerator();
        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(42, enumerator.Current);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await enumerator.MoveNextAsync());
    }
}
