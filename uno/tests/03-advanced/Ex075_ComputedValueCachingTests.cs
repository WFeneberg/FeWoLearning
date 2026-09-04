using FeWoLearning.Uno.Exercises.Advanced;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex075_ComputedValueCachingTests : UnoTestContext
{
    private static Ex075_ComputedValueCaching Range(int minimum = 0, int maximum = 10) =>
        new() { Minimum = minimum, Maximum = maximum };

    [Fact]
    public void The_Value_Is_Computed_On_First_Use()
    {
        var range = Range(2, 8);

        // Nothing yet: computing in the constructor or in the changed callback is work for
        // a value nobody may ever read, and a control's properties are set several times
        // while a page is being built.
        Assert.Equal(0, range.Computations);

        Assert.Equal("2..8", range.RangeText);
        Assert.Equal(1, range.Computations);
    }

    [Fact]
    public void Reading_Again_Reuses_The_Cache()
    {
        var range = Range(2, 8);

        _ = range.RangeText;
        _ = range.RangeText;
        _ = range.RangeText;

        Assert.Equal(1, range.Computations);
    }

    [Fact]
    public void Changing_An_Input_Invalidates_The_Cache()
    {
        var range = Range(2, 8);
        _ = range.RangeText;

        range.Maximum = 9;

        Assert.Equal("2..9", range.RangeText);
        Assert.Equal(2, range.Computations);
    }

    [Fact]
    public void Changing_The_Other_Input_Also_Invalidates()
    {
        var range = Range(2, 8);
        _ = range.RangeText;

        range.Minimum = 1;

        // Forgetting one input is the stale-value failure, and it is invisible until
        // somebody notices the label disagreeing with the slider.
        Assert.Equal("1..8", range.RangeText);
    }

    [Fact]
    public void Changing_Something_That_Is_Not_An_Input_Keeps_The_Cache()
    {
        var range = Range(2, 8);
        _ = range.RangeText;

        range.Label = "anything";
        _ = range.RangeText;

        // The opposite failure: invalidating on everything makes the cache useless while
        // still looking like it works.
        Assert.Equal(1, range.Computations);
    }

    [Fact]
    public void Setting_An_Input_To_Its_Current_Value_Does_Not_Invalidate()
    {
        var range = Range(2, 8);
        _ = range.RangeText;

        range.Maximum = 8;
        _ = range.RangeText;

        // The property system does not raise a change for an unchanged value, so the
        // callback never runs - which the cache gets for free.
        Assert.Equal(1, range.Computations);
    }

    [Fact]
    public void Invalidating_By_Hand_Forces_A_Recompute()
    {
        var range = Range(2, 8);
        _ = range.RangeText;

        range.Invalidate();
        _ = range.RangeText;

        Assert.Equal(2, range.Computations);
    }

    [Fact]
    public void Each_Instance_Caches_Its_Own_Value()
    {
        var first = Range(1, 2);
        var second = Range(3, 4);

        Assert.Equal("1..2", first.RangeText);
        Assert.Equal("3..4", second.RangeText);
        Assert.Equal(1, first.Computations);
    }
}
