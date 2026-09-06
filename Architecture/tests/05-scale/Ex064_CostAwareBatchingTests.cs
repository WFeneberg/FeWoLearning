using FeWoLearning.Architecture.Exercises.Scale.Ex064;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex064_CostAwareBatchingTests
{
    private static readonly TimeSpan MaxAge = TimeSpan.FromSeconds(5);

    private static (Batcher<int> Batcher, ManualClock Clock, List<int[]> Flushed) Build(int maxSize = 3)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var flushed = new List<int[]>();
        return (new Batcher<int>(clock, maxSize, MaxAge, b => flushed.Add([.. b])), clock, flushed);
    }

    [Fact]
    public void A_Full_Batch_Flushes_Immediately()
    {
        var (batcher, _, flushed) = Build();

        batcher.Add(1);
        batcher.Add(2);
        Assert.Empty(flushed);

        batcher.Add(3);

        Assert.Equal([[1, 2, 3]], flushed);
        Assert.Equal(0, batcher.Pending);
    }

    [Fact]
    public void A_Partial_Batch_Waits()
    {
        var (batcher, _, flushed) = Build();

        batcher.Add(1);
        batcher.Add(2);

        Assert.Empty(flushed);
        Assert.Equal(2, batcher.Pending);
    }

    [Fact]
    public void Mechanism_A_Partial_Batch_Flushes_Once_It_Is_Old_Enough()
    {
        // The age trigger is what bounds the worst case. Without it, the last few items of
        // the day sit in the buffer until tomorrow's traffic pushes them out - which looks
        // fine in every load test, because a load test never stops sending.
        var (batcher, clock, flushed) = Build();
        batcher.Add(1);

        clock.Advance(MaxAge - TimeSpan.FromSeconds(1));
        batcher.Tick();
        Assert.Empty(flushed);

        clock.Advance(TimeSpan.FromSeconds(1));
        batcher.Tick();

        Assert.Equal([[1]], flushed);
    }

    [Fact]
    public void Mechanism_The_Age_Is_Measured_From_The_Oldest_Item_Not_The_Last_Flush()
    {
        // The natural mistake, wrong in both directions: it flushes a fresh item early
        // after an idle period, and it resets on every flush, so a steady trickle can
        // leave an item waiting almost 2 x maxAge. Here a long idle gap has passed since
        // the last flush, and the item that was just added must not be swept out with it.
        var (batcher, clock, flushed) = Build();

        batcher.Add(1);
        batcher.Add(2);
        batcher.Add(3);
        Assert.Single(flushed);

        clock.Advance(MaxAge * 10); // a long quiet period
        batcher.Add(4);             // arrives now
        batcher.Tick();

        Assert.Single(flushed);
        Assert.Equal(1, batcher.Pending);

        clock.Advance(MaxAge);
        batcher.Tick();

        Assert.Equal([[1, 2, 3], [4]], flushed);
    }

    [Fact]
    public void Adversarial_An_Empty_Tick_Flushes_Nothing()
    {
        // A flush handler that opens a transaction, or bills per call, must not be invoked
        // to do nothing. An implementation that flushes on every tick regardless passes
        // the age fact above.
        var (batcher, clock, flushed) = Build();

        clock.Advance(MaxAge * 5);
        batcher.Tick();
        batcher.Tick();

        Assert.Empty(flushed);
    }

    [Fact]
    public void An_Explicit_Flush_Empties_A_Partial_Batch()
    {
        var (batcher, _, flushed) = Build();
        batcher.Add(1);

        batcher.Flush();

        Assert.Equal([[1]], flushed);
        Assert.Equal(0, batcher.Pending);
    }

    [Fact]
    public void Flushing_An_Empty_Batcher_Does_Nothing()
    {
        var (batcher, _, flushed) = Build();

        batcher.Flush();
        batcher.Flush();

        Assert.Empty(flushed);
    }
}
