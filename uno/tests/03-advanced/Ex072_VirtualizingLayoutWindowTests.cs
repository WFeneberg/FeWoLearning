using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex072_VirtualizingLayoutWindowTests : UnoTestContext
{
    private static Ex072_VirtualizingLayoutWindow Layout20() => new() { RowHeight = 20 };

    [Fact]
    public void An_Empty_Rect_Covers_Nothing()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, 0, 100, 0), itemCount: 50);

        // The realisation rect really is empty before the first viewport arrives, and a
        // layout that realises "everything" for an empty rect defeats the point.
        Assert.Equal(0, count);
        Assert.Equal(0, first);
    }

    [Fact]
    public void A_Rect_At_The_Top_Covers_The_First_Rows()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, 0, 100, 50), itemCount: 50);

        // 0..50 pixels over 20-pixel rows: rows 0, 1 and 2.
        Assert.Equal(0, first);
        Assert.Equal(3, count);
    }

    [Fact]
    public void A_Scrolled_Rect_Skips_The_Rows_Above_It()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, 100, 100, 40), itemCount: 50);

        Assert.Equal(5, first);
        Assert.Equal(2, count);
    }

    [Fact]
    public void A_Partly_Covered_Row_Is_Still_Realised()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, 10, 100, 20), itemCount: 50);

        // 10..30 touches row 0 and row 1. Rounding the top up would leave a half-drawn row.
        Assert.Equal(0, first);
        Assert.Equal(2, count);
    }

    [Fact]
    public void The_Range_Is_Clamped_To_The_Item_Count()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, 0, 100, 500), itemCount: 3);

        Assert.Equal(0, first);
        Assert.Equal(3, count);
    }

    [Fact]
    public void A_Rect_Past_The_End_Covers_Nothing()
    {
        var (_, count) = Layout20().RangeFor(new Rect(0, 1000, 100, 40), itemCount: 3);

        Assert.Equal(0, count);
    }

    [Fact]
    public void A_Rect_Above_The_Start_Is_Clamped()
    {
        var (first, count) = Layout20().RangeFor(new Rect(0, -50, 100, 70), itemCount: 50);

        // A negative offset happens: the realisation rect is usually grown past the
        // viewport on both sides.
        Assert.Equal(0, first);
        Assert.Equal(1, count);
    }

    [Fact]
    public void The_Extent_Covers_Every_Item_Not_Just_The_Realised_Ones()
    {
        var layout = Layout20();
        // Laid out tall enough that the extent is not clamped by the available height -
        // DesiredSize is capped at the constraint (ex034), which would hide the point.
        var repeater = Layout(
            Ex072_VirtualizingLayoutWindow.CreateRepeater(Enumerable.Range(0, 50).ToArray(), layout),
            width: 100,
            height: 2000);

        // 50 rows of 20 = 1000, whatever was realised. Returning the realised height
        // instead makes the scroll bar shrink as you scroll - the classic symptom.
        Assert.Equal(1000, repeater.DesiredSize.Height, 1);
    }

    [Fact]
    public void The_Layout_Only_Asks_For_What_Its_Rect_Covers()
    {
        var layout = Layout20();
        var repeater = Layout(
            Ex072_VirtualizingLayoutWindow.CreateRepeater(Enumerable.Range(0, 50).ToArray(), layout),
            width: 100,
            height: 60);

        var (first, count) = layout.RangeFor(layout.LastRealizationRect, 50);

        // Whatever rect the harness reported, the layout asked for exactly the indices that
        // rect covers - never all 50.
        Assert.Equal(count, layout.RequestedIndices.Distinct().Count());
        Assert.All(layout.RequestedIndices, index => Assert.InRange(index, first, first + Math.Max(count - 1, 0)));
        Assert.True(repeater.ItemsSourceView.Count == 50);
    }

    [Fact]
    public void An_Empty_Viewport_Realises_Nothing()
    {
        var layout = Layout20();

        Layout(
            Ex072_VirtualizingLayoutWindow.CreateRepeater(Enumerable.Range(0, 5).ToArray(), layout),
            width: 100,
            height: 60);

        // The harness has no viewport, so the rect the repeater reports is empty (see
        // uno/README.md) - and the right response to an empty rect is to realise nothing.
        // A layout that treats "no rect yet" as "show everything" is the one that makes a
        // 50,000-item list hang on the first frame.
        Assert.Equal(0, layout.LastRealizationRect.Height);
        Assert.Empty(layout.RequestedIndices);
    }
}
