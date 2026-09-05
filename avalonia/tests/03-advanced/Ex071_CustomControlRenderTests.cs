using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex071_CustomControlRenderTests
{
    private static Ex071_CustomControlRender WithValues(params double[] values) =>
        new() { Values = values };

    // Point comparison with a tolerance rather than Assert.Equal on the lists.
    // The arithmetic here is legitimately order-sensitive in the last bit -
    // 40 - 2.0 / 3.0 * 40 and 40.0 / 3.0 differ by one ulp - and pinning an exact
    // double would fail a correct answer that grouped the operations differently.
    private static void AssertPoints(Point[] expected, IReadOnlyList<Point> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].X, actual[i].X, precision: 9);
            Assert.Equal(expected[i].Y, actual[i].Y, precision: 9);
        }
    }

    // An ascending series spans the box corner to corner. A mapping that forgets
    // screen y grows downwards produces the vertical mirror of this, which is
    // why the y of every point is pinned rather than just the extremes.
    [AvaloniaFact]
    public void An_Ascending_Series_Runs_Bottom_Left_To_Top_Right()
    {
        var points = WithValues(0, 1, 2, 3).BuildPoints(new Size(90, 40));

        AssertPoints(
            [new Point(0, 40), new Point(30, 80.0 / 3.0), new Point(60, 40.0 / 3.0), new Point(90, 0)],
            points);
    }

    [AvaloniaFact]
    public void A_Descending_Series_Runs_The_Other_Way()
    {
        var points = WithValues(3, 0).BuildPoints(new Size(50, 20));

        AssertPoints([new Point(0, 0), new Point(50, 20)], points);
    }

    // Normalising against the span rather than the maximum: these readings never
    // come near zero, yet the smallest must still land on the bottom edge.
    [AvaloniaFact]
    public void The_Span_Is_What_Is_Normalised_Against_Not_The_Maximum()
    {
        var points = WithValues(100, 110, 120).BuildPoints(new Size(20, 10));

        AssertPoints([new Point(0, 10), new Point(10, 5), new Point(20, 0)], points);
    }

    [AvaloniaFact]
    public void No_Values_Yields_No_Points()
    {
        Assert.Empty(new Ex071_CustomControlRender().BuildPoints(new Size(90, 40)));
    }

    // Both of these divide by zero in the obvious implementation: one value gives
    // a step of Width / 0, and a flat series gives a span of 0.
    [AvaloniaFact]
    public void A_Single_Value_Sits_In_The_Middle()
    {
        var points = WithValues(7).BuildPoints(new Size(80, 40));

        Assert.Equal(new Point(40, 20), Assert.Single(points));
    }

    [AvaloniaFact]
    public void A_Flat_Series_Runs_Across_The_Vertical_Middle()
    {
        var points = WithValues(5, 5, 5).BuildPoints(new Size(60, 30));

        AssertPoints([new Point(0, 15), new Point(30, 15), new Point(60, 15)], points);
    }

    // All that can be said about Render itself. Measured: an exception thrown
    // inside Render surfaces at RunJobs, not at Show, so the drain is what makes
    // this mean anything. It is a weak check by construction - an empty Render
    // body also passes it - and the class header explains why nothing stronger is
    // available in this harness.
    [AvaloniaFact]
    public void Showing_The_Control_Renders_Without_Throwing()
    {
        double[][] series = [[], [4.0], [1.0, 9.0, 3.0], [2.0, 2.0]];

        foreach (var values in series)
        {
            var control = new Ex071_CustomControlRender { Values = values, Width = 90, Height = 40 };
            ViewHarness.Show(control, 120, 60);
            Dispatcher.UIThread.RunJobs();
        }
    }
}
