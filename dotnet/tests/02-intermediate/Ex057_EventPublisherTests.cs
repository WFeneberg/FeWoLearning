using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex057_EventPublisherTests
{
    [Fact]
    public void ThresholdReached_DoesNotFire_BeforeThresholdIsExceeded()
    {
        var counter = new Counter(threshold: 10);
        var raisedCount = 0;
        counter.ThresholdReached += (_, _) => raisedCount++;

        counter.Increment(4);
        counter.Increment(3);
        counter.Increment(3); // Value == 10, must NOT fire (only when EXCEEDED)

        Assert.Equal(10, counter.Value);
        Assert.False(counter.HasReachedThreshold);
        Assert.Equal(0, raisedCount);
    }

    [Fact]
    public void ThresholdReached_FiresExactlyOnce_WhenThresholdIsExceeded()
    {
        var counter = new Counter(threshold: 10);
        var raisedCount = 0;
        int? capturedValue = null;
        counter.ThresholdReached += (sender, args) =>
        {
            raisedCount++;
            capturedValue = args.Value;
        };

        counter.Increment(6);
        counter.Increment(5); // Value == 11 -> crosses threshold here

        Assert.Equal(1, raisedCount);
        Assert.Equal(11, capturedValue);
        Assert.True(counter.HasReachedThreshold);

        // Further increments must not raise the event again.
        counter.Increment(2);
        counter.Increment(100);

        Assert.Equal(1, raisedCount);
        Assert.Equal(113, counter.Value);
    }

    [Fact]
    public void ThresholdReached_PassesCounterAsSender()
    {
        var counter = new Counter(threshold: 1);
        object? sender = null;
        counter.ThresholdReached += (s, _) => sender = s;

        counter.Increment(2);

        Assert.Same(counter, sender);
    }
}
