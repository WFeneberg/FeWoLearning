using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex085_ClippingGeometryTests : UnoTestContext
{
    [Fact]
    public void The_Clip_Carries_Its_Rect()
    {
        var clip = Ex085_ClippingGeometry.CreateClip(new Rect(5, 5, 20, 10));

        Assert.Equal(new Rect(5, 5, 20, 10), clip.Rect);
    }

    [Fact]
    public void The_Clip_Reports_Its_Bounds()
    {
        var clip = Ex085_ClippingGeometry.CreateClip(new Rect(5, 5, 20, 10));

        Assert.Equal(new Rect(5, 5, 20, 10), clip.Bounds);
    }

    [Fact]
    public void Clipping_Attaches_The_Geometry()
    {
        var element = Ex085_ClippingGeometry.Clip(new Border { Width = 40, Height = 40 }, new Rect(0, 0, 10, 10));

        Assert.NotNull(element.Clip);
        Assert.Equal(new Rect(0, 0, 10, 10), element.Clip.Rect);
    }

    [Fact]
    public void Unclipping_Removes_It()
    {
        var element = Ex085_ClippingGeometry.Clip(new Border { Width = 40, Height = 40 }, new Rect(0, 0, 10, 10));

        Ex085_ClippingGeometry.Unclip(element);

        Assert.Null(element.Clip);
    }

    [Fact]
    public void A_Clip_Does_Not_Change_The_Measured_Size()
    {
        var clipped = Layout(Ex085_ClippingGeometry.Clip(new Border { Width = 40, Height = 40 }, new Rect(0, 0, 10, 10)));

        // A clip is a paint-time operation. The element still measures at 40, which is the
        // difference between "it is clipped" and "it is gone".
        Assert.Equal(40, clipped.DesiredSize.Width, 1);
        Assert.Equal(40, clipped.ActualWidth, 1);
    }

    [Fact]
    public void Clipping_To_Nothing_Still_Occupies_The_Layout()
    {
        var panel = new StackPanel();
        panel.Children.Add(Ex085_ClippingGeometry.Clip(new Border { Width = 40, Height = 40 }, new Rect(0, 0, 0, 0)));
        panel.Children.Add(new Border { Width = 40, Height = 10 });

        Layout(panel);

        // 50, not 10: clipping is not collapsing, and a fully clipped element leaves a
        // full-size hole where it was.
        Assert.Equal(50, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Overlapping_Rects_Intersect()
    {
        var visible = Ex085_ClippingGeometry.VisiblePart(new Rect(0, 0, 20, 20), new Rect(10, 5, 20, 20));

        Assert.Equal(new Rect(10, 5, 10, 15), visible);
    }

    [Fact]
    public void A_Contained_Rect_Survives_Whole()
    {
        var visible = Ex085_ClippingGeometry.VisiblePart(new Rect(2, 2, 5, 5), new Rect(0, 0, 20, 20));

        Assert.Equal(new Rect(2, 2, 5, 5), visible);
    }

    [Fact]
    public void Rects_That_Do_Not_Meet_Leave_Nothing()
    {
        var visible = Ex085_ClippingGeometry.VisiblePart(new Rect(0, 0, 5, 5), new Rect(50, 50, 5, 5));

        // Rect.Empty, which is not a zero-sized rect at the origin - its Width is NaN-free
        // but its position is not (0,0), and code that checks `== default` misses it.
        Assert.True(visible.IsEmpty);
    }

    [Fact]
    public void Touching_Edges_Leave_Nothing()
    {
        var visible = Ex085_ClippingGeometry.VisiblePart(new Rect(0, 0, 10, 10), new Rect(10, 0, 10, 10));

        Assert.True(visible.IsEmpty || visible.Width == 0);
    }

    [Fact]
    public void The_Intersection_Does_Not_Mutate_Its_Inputs()
    {
        var content = new Rect(0, 0, 20, 20);
        var clip = new Rect(10, 5, 20, 20);

        Ex085_ClippingGeometry.VisiblePart(content, clip);

        // Rect.Intersect mutates in place, so an implementation that intersects the
        // caller's rect changes a value the caller still holds.
        Assert.Equal(new Rect(0, 0, 20, 20), content);
        Assert.Equal(new Rect(10, 5, 20, 20), clip);
    }
}
