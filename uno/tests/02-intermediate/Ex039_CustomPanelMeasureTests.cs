using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex039_CustomPanelMeasureTests : UnoTestContext
{
    private static Ex039_CustomPanelMeasure Panel(params FrameworkElement[] children)
    {
        var panel = new Ex039_CustomPanelMeasure();
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }

    private static Border Box(double width, double height) => new() { Width = width, Height = height };

    [Fact]
    public void Fills_The_Available_Width()
    {
        var panel = Panel(Box(10, 20), Box(10, 20));

        panel.Measure(new Size(300, 100));

        Assert.Equal(300, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void Is_As_Tall_As_Its_Tallest_Child()
    {
        var panel = Panel(Box(10, 20), Box(10, 55), Box(10, 30));

        panel.Measure(new Size(300, 100));

        Assert.Equal(55, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Hands_Each_Child_One_Column()
    {
        var wide = Box(500, 20);
        var panel = Panel(wide, Box(10, 20), Box(10, 20));

        panel.Measure(new Size(300, 100));

        // The child asked for 500 and was measured with 100, so its DesiredSize is clamped
        // to the promise it was given. That clamp is the whole point of the constraint.
        Assert.Equal(100, wide.DesiredSize.Width, 1);
    }

    [Fact]
    public void A_Single_Child_Gets_Everything()
    {
        var only = Box(500, 20);
        var panel = Panel(only);

        panel.Measure(new Size(300, 100));

        Assert.Equal(300, only.DesiredSize.Width, 1);
    }

    [Fact]
    public void An_Empty_Panel_Asks_For_No_Height()
    {
        var panel = Panel();

        panel.Measure(new Size(300, 100));

        Assert.Equal(0, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void An_Infinite_Width_Does_Not_Become_An_Infinite_Desired_Size()
    {
        var panel = Panel(Box(30, 20), Box(40, 20));

        panel.Measure(new Size(double.PositiveInfinity, 100));

        // A StackPanel or ScrollViewer measures its content with infinity on the stacking
        // axis. Dividing that by the child count is still infinity, and reporting it as a
        // DesiredSize takes the whole layout pass down.
        Assert.False(double.IsInfinity(panel.DesiredSize.Width), "the panel asked for infinite width");
        Assert.Equal(70, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void An_Infinite_Height_Still_Reports_The_Tallest_Child()
    {
        var panel = Panel(Box(30, 20), Box(30, 45));

        panel.Measure(new Size(300, double.PositiveInfinity));

        Assert.Equal(45, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Every_Child_Is_Measured()
    {
        var children = new[] { Box(10, 20), Box(10, 20), Box(10, 20) };
        var panel = Panel(children);

        panel.Measure(new Size(300, 100));

        // A child that is never measured reports a zero DesiredSize and is then arranged
        // into a zero rect - invisible, with no error anywhere.
        Assert.All(children, child => Assert.True(child.DesiredSize.Height > 0));
    }
}
