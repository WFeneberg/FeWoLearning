using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex073_InvalidationTrackingTests : UnoTestContext
{
    private static Ex073_InvalidationTracking Panel()
    {
        var panel = new Ex073_InvalidationTracking();
        panel.Children.Add(new Border { Width = 30, Height = 20 });
        return Layout(panel, width: 200, height: 200);
    }

    [Fact]
    public void A_Size_Affecting_Change_Re_Measures()
    {
        var panel = Panel();

        // The baseline: one pass each from the first layout, and a clean layout after it
        // runs neither again.
        Assert.Equal(1, panel.MeasurePasses);
        Assert.Equal(1, panel.ArrangePasses);
        Layout(panel, width: 200, height: 200);
        Assert.Equal(1, panel.MeasurePasses);

        panel.Gutter = 5;
        Layout(panel, width: 200, height: 200);

        Assert.Equal(2, panel.MeasurePasses);
    }

    [Fact]
    public void A_Size_Affecting_Change_Also_Re_Arranges()
    {
        var panel = Panel();

        panel.Gutter = 5;
        Layout(panel, width: 200, height: 200);

        // Marking measure dirty always costs an arrange too - the new sizes have to be
        // placed. That is why the cheap case below matters.
        Assert.Equal(2, panel.ArrangePasses);
    }

    [Fact]
    public void A_Position_Only_Change_Re_Arranges()
    {
        var panel = Panel();

        panel.Shift = 7;
        Layout(panel, width: 200, height: 200);

        Assert.Equal(2, panel.ArrangePasses);
    }

    [Fact]
    public void A_Position_Only_Change_Does_Not_Re_Measure()
    {
        var panel = Panel();

        panel.Shift = 7;
        Layout(panel, width: 200, height: 200);

        // The whole point: an offset that cannot change any size must not re-measure the
        // subtree. Calling InvalidateMeasure here is the bug that makes an animation
        // re-measure every frame.
        Assert.Equal(1, panel.MeasurePasses);
    }

    [Fact]
    public void The_Gutter_Really_Changes_The_Desired_Size()
    {
        var panel = Panel();

        panel.Gutter = 5;
        Layout(panel, width: 200, height: 200);

        Assert.Equal(40, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void The_Shift_Really_Moves_The_Child()
    {
        var panel = Panel();
        var child = (FrameworkElement)panel.Children[0];

        panel.Shift = 7;
        Layout(panel, width: 200, height: 200);

        Assert.Equal(7, Offset(child).X, 1);
    }
}
