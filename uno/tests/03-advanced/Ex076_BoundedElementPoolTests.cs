using FeWoLearning.Uno.Exercises.Advanced;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex076_BoundedElementPoolTests : UnoTestContext
{
    [Fact]
    public void The_First_Rent_Is_A_Miss()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);

        var element = pool.Rent("apple");

        Assert.Equal("apple", element.Text);
        Assert.Equal(1, pool.Misses);
        Assert.Equal(0, pool.Hits);
    }

    [Fact]
    public void A_Returned_Element_Is_Rented_Again()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);
        var element = pool.Rent("apple");
        pool.Return(element);

        var again = pool.Rent("pear");

        Assert.Same(element, again);
        Assert.Equal("pear", again.Text);
        Assert.Equal(1, pool.Hits);
        Assert.Equal(1, pool.Misses);
    }

    [Fact]
    public void A_Returned_Element_Holds_No_Content()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);
        var element = pool.Rent("apple");

        pool.Return(element);

        // While it waits, the element must not reference what it last showed - otherwise
        // the pool retains that data for as long as the app runs.
        Assert.Equal("", element.Text);
    }

    [Fact]
    public void The_Pool_Never_Exceeds_Its_Capacity()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);
        var elements = new[] { pool.Rent("a"), pool.Rent("b"), pool.Rent("c"), pool.Rent("d") };

        foreach (var element in elements)
        {
            pool.Return(element);
        }

        // A burst of returns is what an unbounded pool grows on: it keeps the high-water
        // mark for ever, and nothing in the code says how big that is.
        Assert.Equal(2, pool.Pooled);
    }

    [Fact]
    public void Returns_Beyond_The_Capacity_Are_Counted()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);
        var elements = new[] { pool.Rent("a"), pool.Rent("b"), pool.Rent("c") };

        foreach (var element in elements)
        {
            pool.Return(element);
        }

        Assert.Equal(1, pool.Evictions);
    }

    [Fact]
    public void An_Evicted_Element_Is_Cleared_Too()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 1);
        var kept = pool.Rent("a");
        var dropped = pool.Rent("b");

        pool.Return(kept);
        pool.Return(dropped);

        // Dropped, not forgotten: whoever handed it back may still hold it, and it must not
        // be the thing that keeps the old item alive.
        Assert.Equal("", dropped.Text);
        Assert.Equal(1, pool.Pooled);
    }

    [Fact]
    public void A_Zero_Capacity_Pool_Pools_Nothing()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 0);

        pool.Return(pool.Rent("a"));

        Assert.Equal(0, pool.Pooled);
        Assert.Equal(1, pool.Evictions);
    }

    [Fact]
    public void Hits_And_Misses_Add_Up_To_The_Rentals()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);

        for (var i = 0; i < 5; i++)
        {
            pool.Return(pool.Rent($"item {i}"));
        }

        // Five rentals: one miss and four hits, which is the number that tells you whether
        // the capacity is right.
        Assert.Equal(5, pool.Hits + pool.Misses);
        Assert.Equal(1, pool.Misses);
    }

    [Fact]
    public void Renting_Beyond_The_Pool_Keeps_Constructing()
    {
        var pool = new Ex076_BoundedElementPool(capacity: 2);
        pool.Return(pool.Rent("a"));

        var first = pool.Rent("b");
        var second = pool.Rent("c");

        // A capped pool does not cap the number of elements in use - only how many are kept
        // when they are not.
        Assert.NotSame(first, second);
        Assert.Equal(2, pool.Misses);
    }
}
