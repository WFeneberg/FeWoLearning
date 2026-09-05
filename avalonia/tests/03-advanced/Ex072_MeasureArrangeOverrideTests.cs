using Avalonia;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Advanced;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex072_MeasureArrangeOverrideTests
{
    private static Size Measured(double availableWidth, double availableHeight, double ratio = 2.0)
    {
        var control = new Ex072_MeasureArrangeOverride { AspectRatio = ratio };
        control.Measure(new Size(availableWidth, availableHeight));
        return control.DesiredSize;
    }

    [AvaloniaTheory]
    [InlineData(200, 200, 200, 100)]
    [InlineData(200, 40, 80, 40)]
    [InlineData(60, 30, 60, 30)]
    [InlineData(50, 200, 50, 25)]
    public void A_Finite_Constraint_Yields_The_Largest_Box_That_Fits(
        double availableWidth, double availableHeight, double expectedWidth, double expectedHeight)
    {
        Assert.Equal(new Size(expectedWidth, expectedHeight), Measured(availableWidth, availableHeight));
    }

    // The case a StackPanel produces on its stacking axis. Deriving the width
    // from an infinite height gives an infinite DesiredSize, which corrupts the
    // parent's arithmetic rather than merely looking wrong.
    [AvaloniaFact]
    public void An_Infinite_Height_Is_Driven_By_The_Finite_Width()
    {
        Assert.Equal(new Size(90, 45), Measured(90, double.PositiveInfinity));
    }

    [AvaloniaFact]
    public void An_Infinite_Width_Is_Driven_By_The_Finite_Height()
    {
        Assert.Equal(new Size(60, 30), Measured(double.PositiveInfinity, 30));
    }

    // What a ScrollViewer measuring both axes produces.
    [AvaloniaFact]
    public void Two_Infinite_Axes_Fall_Back_To_The_Unconstrained_Width()
    {
        var expected = new Size(
            Ex072_MeasureArrangeOverride.UnconstrainedWidth,
            Ex072_MeasureArrangeOverride.UnconstrainedWidth / 2.0);

        Assert.Equal(expected, Measured(double.PositiveInfinity, double.PositiveInfinity));
    }

    // Stated separately from the cases above because it is the invariant rather
    // than an example: whatever the constraint, DesiredSize is finite.
    [AvaloniaFact]
    public void DesiredSize_Is_Never_Infinite()
    {
        var probes = new[]
        {
            Measured(double.PositiveInfinity, double.PositiveInfinity),
            Measured(double.PositiveInfinity, 30),
            Measured(90, double.PositiveInfinity),
        };

        Assert.All(probes, size =>
        {
            Assert.False(double.IsInfinity(size.Width));
            Assert.False(double.IsInfinity(size.Height));
        });
    }

    // Guards against an answer that hard-codes the default ratio of 2.
    [AvaloniaFact]
    public void The_Ratio_Is_Honoured_Rather_Than_Hard_Coded()
    {
        Assert.Equal(new Size(50, 200), Measured(200, 200, ratio: 0.25));
        Assert.Equal(new Size(200, 25), Measured(200, 200, ratio: 8));
    }

    // ArrangeOverride's half: reporting the size used is what fixes Bounds, so
    // returning default or DesiredSize leaves the control the wrong size.
    [AvaloniaFact]
    public void Arrange_Commits_To_The_Size_The_Parent_Gave()
    {
        var control = new Ex072_MeasureArrangeOverride();

        control.Measure(new Size(200, 200));
        control.Arrange(new Rect(0, 0, 120, 60));

        Assert.Equal(new Rect(0, 0, 120, 60), control.Bounds);
    }
}
