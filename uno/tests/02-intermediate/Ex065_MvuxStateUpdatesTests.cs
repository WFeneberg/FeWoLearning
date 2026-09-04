using FeWoLearning.Uno.Exercises.Intermediate;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex065_MvuxStateUpdatesTests : UnoTestContext
{
    private static ValueTask<int> Current(Ex065_Counter counter) =>
        Ex065_MvuxStateUpdates.CurrentAsync(counter.Count, CancellationToken.None);

    [Fact]
    public async Task The_State_Starts_At_Its_Seed()
    {
        var counter = new Ex065_Counter(seed: 1);

        Assert.Equal(1, await Current(counter));
    }

    [Fact]
    public void The_State_Is_The_Same_Instance_Every_Time()
    {
        var counter = new Ex065_Counter(seed: 1);

        // Keyed by owner. A new state per call would leave every reader watching a
        // different one, and updates would appear to do nothing.
        Assert.Same(counter.Count, counter.Count);
    }

    [Fact]
    public async Task Incrementing_Raises_The_Value()
    {
        var counter = new Ex065_Counter(seed: 1);

        await counter.IncrementAsync(CancellationToken.None);

        Assert.Equal(2, await Current(counter));
    }

    [Fact]
    public async Task Increments_Accumulate()
    {
        var counter = new Ex065_Counter(seed: 0);

        await counter.IncrementAsync(CancellationToken.None);
        await counter.IncrementAsync(CancellationToken.None);
        await counter.IncrementAsync(CancellationToken.None);

        Assert.Equal(3, await Current(counter));
    }

    [Fact]
    public async Task Resetting_Returns_To_The_Seed()
    {
        var counter = new Ex065_Counter(seed: 5);
        await counter.IncrementAsync(CancellationToken.None);

        await counter.ResetAsync(CancellationToken.None);

        Assert.Equal(5, await Current(counter));
    }

    [Fact]
    public async Task Every_Update_Reaches_The_Stream()
    {
        var counter = new Ex065_Counter(seed: 1);

        var messages = await MvuxObserver.Collect(counter.Count, 3, async () =>
        {
            await counter.IncrementAsync(CancellationToken.None);
            await counter.IncrementAsync(CancellationToken.None);
        });

        // The stream is the trustworthy view: seed, then each update, in order.
        Assert.Equal([1, 2, 3], MvuxObserver.ValuesOf(messages));
    }

    [Fact]
    public async Task Two_Counters_Own_Two_States()
    {
        var first = new Ex065_Counter(seed: 1);
        var second = new Ex065_Counter(seed: 100);

        await first.IncrementAsync(CancellationToken.None);

        Assert.Equal(2, await Current(first));
        Assert.Equal(100, await Current(second));
    }

    [Fact]
    public async Task The_Same_Read_Outside_A_Context_Is_Current()
    {
        var counter = new Ex065_Counter(seed: 1);

        await counter.IncrementAsync(CancellationToken.None);

        // No context, so the read materialises the state as it is now. This is the shape
        // CurrentAsync has to have - and the reason it must not open a context itself.
        Assert.Equal(2, await Current(counter));
    }
}
