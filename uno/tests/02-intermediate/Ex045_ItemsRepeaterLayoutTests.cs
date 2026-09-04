using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex045_ItemsRepeaterLayoutTests : UnoTestContext
{
    private static readonly string[] FiveItems = ["a", "b", "c", "d", "e"];

    private static ItemsRepeater Repeater(int columns = 2, int count = 5, double rowHeight = 20)
    {
        var layout = new Ex045_ItemsRepeaterLayout { Columns = columns, RowHeight = rowHeight };
        return Layout(
            Ex045_ItemsRepeaterLayout.CreateRepeater(FiveItems[..count], layout),
            width: 200,
            height: 400);
    }

    private static FrameworkElement Element(ItemsRepeater repeater, int index) =>
        (FrameworkElement)repeater.TryGetElement(index)!;

    [Fact]
    public void Realises_One_Element_Per_Item()
    {
        var repeater = Repeater();

        Assert.All(
            Enumerable.Range(0, 5),
            i => Assert.NotNull(repeater.TryGetElement(i)));
    }

    [Fact]
    public void Fills_The_First_Row_Left_To_Right()
    {
        var repeater = Repeater(columns: 2);

        Assert.Equal(0, Offset(Element(repeater, 0)).X, 1);
        Assert.Equal(100, Offset(Element(repeater, 1)).X, 1);
        Assert.Equal(0, Offset(Element(repeater, 0)).Y, 1);
        Assert.Equal(0, Offset(Element(repeater, 1)).Y, 1);
    }

    [Fact]
    public void Wraps_After_The_Last_Column()
    {
        var repeater = Repeater(columns: 2);

        Assert.Equal(0, Offset(Element(repeater, 2)).X, 1);
        Assert.Equal(20, Offset(Element(repeater, 2)).Y, 1);
    }

    [Fact]
    public void Honours_A_Different_Column_Count()
    {
        var repeater = Repeater(columns: 4);

        // 200 wide over 4 columns: 50 each, and the wrap lands at index 4.
        Assert.Equal(150, Offset(Element(repeater, 3)).X, 1);
        Assert.Equal(0, Offset(Element(repeater, 4)).X, 1);
        Assert.Equal(20, Offset(Element(repeater, 4)).Y, 1);
    }

    [Fact]
    public void Honours_A_Different_Row_Height()
    {
        var repeater = Repeater(columns: 2, rowHeight: 35);

        Assert.Equal(35, Offset(Element(repeater, 2)).Y, 1);
    }

    [Fact]
    public void Asks_For_As_Many_Rows_As_It_Needs()
    {
        var repeater = Repeater(columns: 2);

        // Five items in two columns is three rows, not two and a half.
        Assert.Equal(60, repeater.DesiredSize.Height, 1);
    }

    [Fact]
    public void A_Full_Last_Row_Is_Not_Rounded_Up()
    {
        var repeater = Repeater(columns: 2, count: 4);

        Assert.Equal(40, repeater.DesiredSize.Height, 1);
    }

    [Fact]
    public void An_Empty_Source_Needs_No_Rows()
    {
        var layout = new Ex045_ItemsRepeaterLayout();
        var repeater = Layout(
            Ex045_ItemsRepeaterLayout.CreateRepeater(Array.Empty<string>(), layout),
            width: 200,
            height: 400);

        Assert.Equal(0, repeater.DesiredSize.Height, 1);
    }

    [Fact]
    public void Swapping_The_Layout_Re_Arranges_The_Same_Elements()
    {
        var repeater = Repeater(columns: 2);

        repeater.Layout = new Ex045_ItemsRepeaterLayout { Columns = 5, RowHeight = 20 };
        Layout(repeater, width: 200, height: 400);

        // A Layout owns no children - it asks the context for them. That is what lets the
        // repeater keep its elements across a layout swap instead of rebuilding them.
        Assert.Equal(0, Offset(Element(repeater, 4)).Y, 1);
        Assert.Equal(160, Offset(Element(repeater, 4)).X, 1);
    }

    [Fact]
    public void Each_Element_Is_Measured_To_Its_Cell()
    {
        var repeater = Repeater(columns: 2);

        // The template asks for 10x10 and the cell offers 100x20, so the element keeps its
        // own desired size - the cell is a constraint, not an assignment.
        Assert.Equal(10, Element(repeater, 0).DesiredSize.Width, 1);
    }
}
