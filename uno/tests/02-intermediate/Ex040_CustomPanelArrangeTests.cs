using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex040_CustomPanelArrangeTests : UnoTestContext
{
    private static Ex040_CustomPanelArrange Panel(params FrameworkElement[] children)
    {
        var panel = new Ex040_CustomPanelArrange();
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }

    private static Border Box(double size = 30) => new() { Width = size, Height = size };

    [Fact]
    public void Puts_The_First_Child_At_The_Origin()
    {
        var first = Box();
        Layout(Panel(first, Box()), width: 200, height: 200);

        Assert.Equal(0, Offset(first).X, 1);
        Assert.Equal(0, Offset(first).Y, 1);
    }

    [Fact]
    public void Shifts_Each_Child_On_Both_Axes()
    {
        var second = Box();
        var third = Box();
        Layout(Panel(Box(), second, third), width: 200, height: 200);

        Assert.Equal(10, Offset(second).X, 1);
        Assert.Equal(10, Offset(second).Y, 1);
        Assert.Equal(20, Offset(third).X, 1);
        Assert.Equal(20, Offset(third).Y, 1);
    }

    [Fact]
    public void Honours_A_Changed_Offset()
    {
        var second = Box();
        var panel = Panel(Box(), second);
        panel.Offset = 25;

        Layout(panel, width: 200, height: 200);

        Assert.Equal(25, Offset(second).X, 1);
    }

    [Fact]
    public void Arranges_Children_At_Their_Own_Size()
    {
        var small = Box(15);
        var large = Box(45);

        Layout(Panel(small, large), width: 300, height: 300);

        // Not at the panel's size, and not stretched: the Rect handed to a child is the
        // final word on how big it is.
        Assert.Equal(15, small.ActualWidth, 1);
        Assert.Equal(45, large.ActualWidth, 1);
    }

    [Fact]
    public void Reports_The_Bounding_Box_It_Used()
    {
        var panel = Panel(Box(), Box(), Box());

        Layout(panel, width: 200, height: 200);

        // Three 30-wide boxes at 0, 10 and 20: the diagonal covers 50, not the 200 the
        // panel was offered.
        Assert.Equal(50, panel.ActualWidth, 1);
        Assert.Equal(50, panel.ActualHeight, 1);
    }

    [Fact]
    public void Children_May_Be_Arranged_Past_The_Panel()
    {
        var last = Box();
        var panel = Panel(Box(), Box(), last);

        // Deliberately smaller than the diagonal needs.
        Layout(panel, width: 25, height: 25);

        // Arrange is not clamped for you. The child sits at 20 in a 25-wide panel and
        // reaches to 50 - honoured, and clipped only if an ancestor clips.
        Assert.Equal(20, Offset(last).X, 1);
        Assert.Equal(30, last.ActualWidth, 1);
    }

    [Fact]
    public void An_Empty_Panel_Arranges_To_Nothing()
    {
        var panel = Panel();

        Layout(panel, width: 200, height: 200);

        Assert.Equal(0, panel.ActualWidth, 1);
    }

    [Fact]
    public void Every_Child_Is_Arranged()
    {
        var children = new[] { Box(), Box(), Box() };

        Layout(Panel(children), width: 300, height: 300);

        // An unarranged child keeps a zero ActualWidth however well it was measured.
        Assert.All(children, child => Assert.Equal(30, child.ActualWidth, 1));
    }
}
