using FeWoLearning.Architecture.Exercises.Scale.Ex063;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex063_BackpressureBoundedQueueTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    private static List<int> Drain(BoundedBuffer<int> buffer)
    {
        var drained = new List<int>();
        while (buffer.TryRead(out var item))
            drained.Add(item);
        return drained;
    }

    [Fact]
    public void Writes_Up_To_Capacity_Are_Accepted_And_Read_Back_In_Order()
    {
        var buffer = new BoundedBuffer<int>(3, FullPolicy.DropNewest);

        Assert.True(buffer.TryWrite(1));
        Assert.True(buffer.TryWrite(2));
        Assert.True(buffer.TryWrite(3));

        Assert.Equal(3, buffer.Count);
        Assert.Equal(0, buffer.Dropped);
        Assert.Equal([1, 2, 3], Drain(buffer));
    }

    [Fact]
    public void Mechanism_DropNewest_Refuses_The_Arrival_And_Keeps_What_Was_Queued()
    {
        // The right answer for events: the first ones say what started, and losing the
        // middle of a sequence is worse than losing its tail.
        var buffer = new BoundedBuffer<int>(3, FullPolicy.DropNewest);
        for (var i = 1; i <= 3; i++) buffer.TryWrite(i);

        Assert.False(buffer.TryWrite(4));

        Assert.Equal([1, 2, 3], Drain(buffer));
        Assert.Equal(1, buffer.Dropped);
    }

    [Fact]
    public void Mechanism_DropOldest_Accepts_The_Arrival_And_Discards_The_Stale_Value()
    {
        // The right answer for state - the current temperature, the latest position -
        // where a stale value has no worth at all. Paired with the fact above because
        // either policy alone is satisfied by "the buffer never exceeds capacity"; only
        // the two together show WHICH item went.
        var buffer = new BoundedBuffer<int>(3, FullPolicy.DropOldest);
        for (var i = 1; i <= 3; i++) buffer.TryWrite(i);

        Assert.True(buffer.TryWrite(4));

        Assert.Equal([2, 3, 4], Drain(buffer));
        Assert.Equal(1, buffer.Dropped);
    }

    [Fact]
    public void Adversarial_The_Buffer_Never_Exceeds_Its_Capacity()
    {
        // An unbounded queue is not the absence of a policy, it is a policy: "grow until
        // the process runs out of memory, then lose everything at once". It passes any
        // fact that only checks items come back out.
        var dropNewest = new BoundedBuffer<int>(2, FullPolicy.DropNewest);
        var dropOldest = new BoundedBuffer<int>(2, FullPolicy.DropOldest);

        for (var i = 0; i < 100; i++)
        {
            dropNewest.TryWrite(i);
            dropOldest.TryWrite(i);
        }

        Assert.Equal(2, dropNewest.Count);
        Assert.Equal(2, dropOldest.Count);
        Assert.Equal(98, dropNewest.Dropped);
        Assert.Equal(98, dropOldest.Dropped);
    }

    [Fact]
    public async Task Mechanism_Wait_Blocks_The_Producer_Until_A_Read_Frees_A_Slot()
    {
        // Backpressure reaching all the way up: the producer is made to slow down rather
        // than the buffer being made to grow. An implementation that quietly accepts the
        // write passes every count assertion and has no bound at all.
        var buffer = new BoundedBuffer<int>(2, FullPolicy.Wait);

        await buffer.WriteAsync(1);
        await buffer.WriteAsync(2);

        var blocked = buffer.WriteAsync(3);

        Assert.False(blocked.IsCompleted);
        Assert.Equal(2, buffer.Count);

        Assert.True(buffer.TryRead(out var first));
        Assert.Equal(1, first);

        await blocked.WaitAsync(Patience);
        Assert.Equal([2, 3], Drain(buffer));
    }

    [Fact]
    public void Reading_An_Empty_Buffer_Reports_It_Rather_Than_Throwing()
    {
        var buffer = new BoundedBuffer<int>(2, FullPolicy.DropNewest);

        Assert.False(buffer.TryRead(out _));
    }

    [Fact]
    public void Adversarial_Dropping_Is_Counted_Rather_Than_Silent()
    {
        // Silent loss is the failure mode that outlives everybody who understood the
        // system. Dropped is a number somebody can alert on; without it, the only symptom
        // is that a report is slightly wrong once a week.
        var buffer = new BoundedBuffer<int>(1, FullPolicy.DropNewest);
        buffer.TryWrite(1);

        buffer.TryWrite(2);
        buffer.TryWrite(3);

        Assert.Equal(2, buffer.Dropped);
    }
}
