using FeWoLearning.Architecture.Exercises.Runtime.Ex093;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex093_ObjectPoolReuseTests
{
    [Fact]
    public void A_Returned_Buffer_Is_Rented_Again()
    {
        var pool = new BufferPool(capacity: 4);

        var first = pool.Rent();
        pool.Return(first);
        var second = pool.Rent();

        Assert.Same(first, second);
        Assert.Equal(2, second.TimesRented);
        Assert.Equal(1, pool.Created);
    }

    [Fact]
    public void Mechanism_A_Rented_Buffer_Carries_Nothing_From_The_Last_Caller()
    {
        // How one customer's invoice gets another customer's address - and nothing
        // anywhere throws. The reuse fact above passes perfectly without this one.
        var pool = new BufferPool(capacity: 4);

        var first = pool.Rent();
        first.Title = "Invoice for Acme";
        first.Lines.Add("1 Main St, Springfield");
        pool.Return(first);

        var second = pool.Rent();

        Assert.Null(second.Title);
        Assert.Empty(second.Lines);
    }

    [Fact]
    public void Mechanism_The_Reset_Happens_On_Return_Rather_Than_On_Rent()
    {
        // A dirty buffer sitting in the pool holds every string it captured alive until
        // somebody happens to rent it - a leak that looks exactly like normal usage from a
        // memory profile. Observable here as the buffer already being clean while it sits
        // idle.
        var pool = new BufferPool(capacity: 4);
        var buffer = pool.Rent();
        buffer.Title = "Invoice for Acme";
        buffer.Lines.Add("1 Main St, Springfield");

        pool.Return(buffer);

        Assert.Null(buffer.Title);
        Assert.Empty(buffer.Lines);
    }

    [Fact]
    public void Mechanism_Renting_Beyond_The_Pool_Allocates_Rather_Than_Failing()
    {
        // The difference from exercise 092, and it runs the other way. A connection pool
        // bounds a scarce external resource, so exhaustion must be an error; an object pool
        // bounds nothing, so exhaustion just means "allocate one". Getting the two
        // backwards gives you either unlimited connections or a buffer pool that throws
        // under load.
        var pool = new BufferPool(capacity: 2);

        var buffers = Enumerable.Range(0, 5).Select(_ => pool.Rent()).ToList();

        Assert.Equal(5, buffers.Distinct().Count());
        Assert.Equal(5, pool.Created);
    }

    [Fact]
    public void Adversarial_The_Pool_Does_Not_Keep_More_Than_Its_Capacity()
    {
        // Keeping everything returned makes this an unbounded cache of objects nobody is
        // using - which is the opposite of the point, and grows with peak load rather than
        // with steady load.
        var pool = new BufferPool(capacity: 2);
        var buffers = Enumerable.Range(0, 5).Select(_ => pool.Rent()).ToList();

        foreach (var buffer in buffers)
            pool.Return(buffer);

        Assert.Equal(2, pool.Idle);
    }

    [Fact]
    public void A_Fresh_Buffer_Starts_Empty()
    {
        var pool = new BufferPool(capacity: 1);

        var buffer = pool.Rent();

        Assert.Null(buffer.Title);
        Assert.Empty(buffer.Lines);
        Assert.Equal(1, buffer.TimesRented);
    }
}
