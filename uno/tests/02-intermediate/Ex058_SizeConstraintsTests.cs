using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex058_SizeConstraintsTests : UnoTestContext
{
    private static double Width(double? width = null, double? min = null, double? max = null, double available = 200) =>
        Ex058_SizeConstraints.ResolveWidth(width, min, max, available);

    [Fact]
    public void An_Explicit_Width_Is_Used_As_Is()
    {
        Assert.Equal(30, Width(width: 30), 1);
    }

    [Fact]
    public void With_No_Width_The_Element_Stretches()
    {
        Assert.Equal(200, Width(), 1);
    }

    [Fact]
    public void A_Maximum_Caps_The_Stretch()
    {
        Assert.Equal(50, Width(max: 50), 1);
    }

    [Fact]
    public void A_Maximum_Caps_An_Explicit_Width()
    {
        Assert.Equal(100, Width(width: 900, max: 100), 1);
    }

    [Fact]
    public void A_Minimum_Raises_An_Explicit_Width()
    {
        // The minimum is applied last, so it wins over the Width somebody asked for.
        Assert.Equal(40, Width(width: 10, min: 40), 1);
    }

    [Fact]
    public void A_Minimum_Can_Exceed_The_Available_Space()
    {
        // 300 in a 200-wide slot, honoured. This is the overflow everybody reports as a
        // layout bug: nothing clamps a minimum to the container.
        Assert.Equal(300, Width(min: 300), 1);
    }

    [Fact]
    public void A_Width_Inside_The_Range_Is_Left_Alone()
    {
        Assert.Equal(30, Width(width: 30, min: 10, max: 100), 1);
    }

    [Fact]
    public void A_Minimum_Above_The_Maximum_Still_Wins()
    {
        // max(MinWidth, min(MaxWidth, Width)): contradictory constraints resolve to the
        // minimum, not to the maximum and not to an exception.
        Assert.Equal(80, Width(min: 80, max: 20), 1);
    }

    [Fact]
    public void Unset_Is_Not_Zero()
    {
        // An unset MaxWidth is infinity, not 0. Passing 0 for "not set" would collapse the
        // element and look like a measure bug.
        Assert.Equal(200, Width(min: null, max: null), 1);
    }

    [Fact]
    public void The_Available_Space_Changes_A_Stretched_Answer_Only()
    {
        Assert.Equal(120, Width(available: 120), 1);
        Assert.Equal(30, Width(width: 30, available: 120), 1);
    }
}
