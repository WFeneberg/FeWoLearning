using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex041_WrapPanelTests : UnoTestContext
{
    private static Border Box(double width = 30, double height = 20) => new() { Width = width, Height = height };

    private static Ex041_WrapPanel Panel(params FrameworkElement[] children)
    {
        var panel = new Ex041_WrapPanel();
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }

    [Fact]
    public void Children_That_Fit_Stay_On_One_Line()
    {
        var second = Box();
        var panel = Panel(Box(), second);

        Layout(panel, width: 100, height: 100);

        Assert.Equal(30, Offset(second).X, 1);
        Assert.Equal(0, Offset(second).Y, 1);
    }

    [Fact]
    public void A_Child_That_Does_Not_Fit_Starts_A_New_Line()
    {
        var third = Box();
        var panel = Panel(Box(), Box(), third);

        Layout(panel, width: 70, height: 100);

        // Two 30-wide boxes fit in 70; the third would reach 90.
        Assert.Equal(0, Offset(third).X, 1);
        Assert.Equal(20, Offset(third).Y, 1);
    }

    [Fact]
    public void Reports_The_Height_Of_All_Its_Lines()
    {
        var panel = Panel(Box(), Box(), Box());

        panel.Measure(new Size(70, 500));

        Assert.Equal(40, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void A_Line_Is_As_Tall_As_Its_Tallest_Child()
    {
        var wrapped = Box();
        var panel = Panel(Box(30, 20), Box(30, 45), wrapped);

        Layout(panel, width: 70, height: 200);

        // The first line is 45 tall because of its tallest member, so the wrapped child
        // starts there and not at 20.
        Assert.Equal(45, Offset(wrapped).Y, 1);
    }

    [Fact]
    public void Reports_The_Width_Of_Its_Widest_Line()
    {
        var panel = Panel(Box(), Box(), Box());

        panel.Measure(new Size(70, 500));

        // Two boxes on the first line, one on the second: 60, not 70 and not 90.
        Assert.Equal(60, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void Measure_And_Arrange_Agree_On_The_Line_Breaks()
    {
        var panel = Panel(Box(), Box(), Box(), Box(), Box());

        Layout(panel, width: 70, height: 500);

        // The height the measure pass promised is the height the arrange pass used. When
        // the two disagree the panel claims space it does not fill, and the layout jitters
        // as soon as anything above it re-measures.
        Assert.Equal(panel.DesiredSize.Height, 60, 1);
    }

    [Fact]
    public void A_Child_Wider_Than_The_Line_Gets_Its_Own_Line()
    {
        var wide = Box(200, 20);
        var after = Box();
        var panel = Panel(Box(), wide, after);

        Layout(panel, width: 70, height: 200);

        // No line can hold it, and a break-until-it-fits loop would never terminate.
        Assert.Equal(0, Offset(wide).X, 1);
        Assert.Equal(20, Offset(wide).Y, 1);
        Assert.Equal(40, Offset(after).Y, 1);
    }

    [Fact]
    public void Arranges_Children_At_Their_Own_Size()
    {
        var box = Box(30, 20);

        Layout(Panel(box), width: 200, height: 200);

        Assert.Equal(30, box.ActualWidth, 1);
        Assert.Equal(20, box.ActualHeight, 1);
    }

    [Fact]
    public void An_Empty_Panel_Needs_Nothing()
    {
        var panel = Panel();

        panel.Measure(new Size(100, 100));

        Assert.Equal(0, panel.DesiredSize.Width, 1);
        Assert.Equal(0, panel.DesiredSize.Height, 1);
    }
}
