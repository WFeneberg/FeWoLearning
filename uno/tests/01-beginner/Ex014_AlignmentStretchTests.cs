using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex014_AlignmentStretchTests : UnoTestContext
{
    private static Border Sized() => new() { Width = 40, Height = 20 };

    private static Border Place(Border child, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
        Layout(Ex014_AlignmentStretch.CreateCell(child, horizontal, vertical), width: 200, height: 100);
        return child;
    }

    [Fact]
    public void Top_Left_Sits_At_The_Origin()
    {
        var child = Place(Sized(), HorizontalAlignment.Left, VerticalAlignment.Top);

        Assert.Equal(0, Offset(child).X, 1);
        Assert.Equal(0, Offset(child).Y, 1);
    }

    [Fact]
    public void Bottom_Right_Sits_Against_The_Far_Edges()
    {
        var child = Place(Sized(), HorizontalAlignment.Right, VerticalAlignment.Bottom);

        Assert.Equal(160, Offset(child).X, 1);
        Assert.Equal(80, Offset(child).Y, 1);
    }

    [Fact]
    public void Centre_Splits_The_Leftover_Space()
    {
        var child = Place(Sized(), HorizontalAlignment.Center, VerticalAlignment.Center);

        Assert.Equal(80, Offset(child).X, 1);
        Assert.Equal(40, Offset(child).Y, 1);
    }

    [Fact]
    public void Stretch_Without_A_Size_Fills_The_Cell()
    {
        var child = Place(new Border(), HorizontalAlignment.Stretch, VerticalAlignment.Stretch);

        Assert.Equal(200, child.ActualWidth, 1);
        Assert.Equal(100, child.ActualHeight, 1);
        Assert.Equal(0, Offset(child).X, 1);
    }

    [Fact]
    public void Stretch_With_A_Size_Centres_Instead_Of_Filling()
    {
        var child = Place(Sized(), HorizontalAlignment.Stretch, VerticalAlignment.Stretch);

        // The rule that costs everyone an afternoon: an explicit Width wins over Stretch,
        // and the leftover space is then split - the element does not stay at the origin.
        Assert.Equal(40, child.ActualWidth, 1);
        Assert.Equal(80, Offset(child).X, 1);
        Assert.Equal(40, Offset(child).Y, 1);
    }

    [Fact]
    public void Mixed_Alignments_Are_Independent()
    {
        var child = Place(Sized(), HorizontalAlignment.Right, VerticalAlignment.Top);

        Assert.Equal(160, Offset(child).X, 1);
        Assert.Equal(0, Offset(child).Y, 1);
    }

    [Fact]
    public void Applies_The_Requested_Alignments_To_The_Child_Itself()
    {
        var child = Sized();

        Ex014_AlignmentStretch.CreateCell(child, HorizontalAlignment.Right, VerticalAlignment.Bottom);

        // Alignment is the child's property, not something the parent remembers about it.
        Assert.Equal(HorizontalAlignment.Right, child.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Bottom, child.VerticalAlignment);
    }
}
