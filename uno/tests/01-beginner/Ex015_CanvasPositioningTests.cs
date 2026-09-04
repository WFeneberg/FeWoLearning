using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex015_CanvasPositioningTests : UnoTestContext
{
    private static (Canvas Canvas, Border Back, Border Front) Scene()
    {
        var back = new Border { Width = 60, Height = 30 };
        var front = new Border { Width = 20, Height = 20 };
        return (Ex015_CanvasPositioning.CreateScene(back, front), back, front);
    }

    [Fact]
    public void Adds_Both_Children_In_The_Documented_Order()
    {
        var (canvas, back, front) = Scene();

        Assert.Equal(2, canvas.Children.Count);
        Assert.Same(back, canvas.Children[0]);
        Assert.Same(front, canvas.Children[1]);
    }

    [Fact]
    public void Records_The_Positions_As_Attached_Values()
    {
        var (_, back, front) = Scene();

        Assert.Equal(10, Canvas.GetLeft(back));
        Assert.Equal(20, Canvas.GetTop(back));
        Assert.Equal(120, Canvas.GetLeft(front));
        Assert.Equal(40, Canvas.GetTop(front));
    }

    [Fact]
    public void Puts_Children_Exactly_Where_It_Was_Told()
    {
        var (canvas, back, front) = Scene();

        Layout(canvas, width: 300, height: 200);

        Assert.Equal(10, Offset(back).X, 1);
        Assert.Equal(20, Offset(back).Y, 1);
        Assert.Equal(120, Offset(front).X, 1);
        Assert.Equal(40, Offset(front).Y, 1);
    }

    [Fact]
    public void Z_Index_Paints_The_Later_Child_Underneath()
    {
        var (_, back, front) = Scene();

        // "back" was added first and still paints on top, because ZIndex outranks the
        // document order the other panels rely on.
        Assert.Equal(1, Canvas.GetZIndex(back));
        Assert.Equal(0, Canvas.GetZIndex(front));
    }

    [Fact]
    public void Children_Keep_Their_Own_Desired_Size()
    {
        var (canvas, back, front) = Scene();

        Layout(canvas, width: 300, height: 200);

        // A Canvas measures with infinite space: nothing is stretched, nothing is squeezed.
        Assert.Equal(60, back.ActualWidth, 1);
        Assert.Equal(20, front.ActualWidth, 1);
    }

    [Fact]
    public void The_Canvas_Itself_Asks_For_Nothing()
    {
        var (canvas, _, _) = Scene();

        Layout(canvas, width: 300, height: 200);

        // Children do not contribute to a Canvas's DesiredSize, so a Canvas inside an Auto
        // row collapses to nothing - a classic "my canvas is invisible" report.
        Assert.Equal(0, canvas.DesiredSize.Width, 1);
        Assert.Equal(0, canvas.DesiredSize.Height, 1);
    }
}
