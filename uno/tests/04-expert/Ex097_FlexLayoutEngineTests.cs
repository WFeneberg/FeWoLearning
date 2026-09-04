using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex097_FlexLayoutEngineTests : UnoTestContext
{
    /// <summary>
    /// A fixed child carries its own Width and aligns left, so it keeps that width. A
    /// growing child carries none and stretches, so its ActualWidth is the slot the panel
    /// arranged it into - an explicit Width would win over the slot (ex058) and hide the
    /// whole exercise.
    /// </summary>
    private static Border Child(double width, int grow)
    {
        var child = grow > 0
            ? new Border { Height = 10, HorizontalAlignment = HorizontalAlignment.Stretch }
            : new Border { Width = width, Height = 10, HorizontalAlignment = HorizontalAlignment.Left };

        Ex097_FlexLayoutEngine.SetGrow(child, grow);
        return child;
    }

    private static Ex097_FlexLayoutEngine Flex(params Border[] children)
    {
        var panel = new Ex097_FlexLayoutEngine();
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }

    [Fact]
    public void Children_Without_A_Grow_Factor_Keep_Their_Width()
    {
        var first = Child(30, grow: 0);
        var second = Child(50, grow: 0);

        Layout(Flex(first, second), width: 200, height: 50);

        Assert.Equal(30, first.ActualWidth, 1);
        Assert.Equal(50, second.ActualWidth, 1);
    }

    [Fact]
    public void A_Growing_Child_Takes_The_Leftover()
    {
        var fixedChild = Child(30, grow: 0);
        var growing = Child(10, grow: 1);

        Layout(Flex(fixedChild, growing), width: 200, height: 50);

        Assert.Equal(170, growing.ActualWidth, 1);
    }

    [Fact]
    public void Two_Growing_Children_Share_By_Weight()
    {
        var fixedChild = Child(20, grow: 0);
        var single = Child(10, grow: 1);
        var double_ = Child(10, grow: 2);

        Layout(Flex(fixedChild, single, double_), width: 200, height: 50);

        // 180 left, split 1:2. Applying the factors in one pass would give the first child
        // everything, because the fixed widths are not known yet.
        Assert.Equal(60, single.ActualWidth, 1);
        Assert.Equal(120, double_.ActualWidth, 1);
    }

    [Fact]
    public void The_Children_Sit_Side_By_Side()
    {
        var first = Child(30, grow: 0);
        var second = Child(10, grow: 1);

        Layout(Flex(first, second), width: 200, height: 50);

        Assert.Equal(0, Offset(first).X, 1);
        Assert.Equal(30, Offset(second).X, 1);
    }

    [Fact]
    public void No_Leftover_Means_No_Growth()
    {
        var fixedChild = Child(200, grow: 0);
        var growing = Child(10, grow: 1);

        Layout(Flex(fixedChild, growing), width: 200, height: 50);

        // The leftover is negative and clamped to zero, so the growing child gets nothing
        // rather than a negative width - which throws inside Measure.
        Assert.Equal(0, growing.ActualWidth, 1);
    }

    [Fact]
    public void Over_Subscribed_Fixed_Children_Are_Not_Shrunk()
    {
        var first = Child(150, grow: 0);
        var second = Child(150, grow: 0);

        Layout(Flex(first, second), width: 200, height: 50);

        // Flexbox would shrink them; this engine does not, and says so by overflowing.
        Assert.Equal(150, first.ActualWidth, 1);
        Assert.Equal(150, second.ActualWidth, 1);
    }

    [Fact]
    public void The_Panel_Fills_The_Available_Width()
    {
        var panel = Flex(Child(30, 0), Child(10, 1));

        panel.Measure(new Windows.Foundation.Size(200, 50));

        Assert.Equal(200, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void The_Panel_Is_As_Tall_As_Its_Row()
    {
        var panel = Flex(Child(30, 0));
        panel.RowHeight = 35;

        panel.Measure(new Windows.Foundation.Size(200, 50));

        Assert.Equal(35, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void An_Empty_Panel_Is_Harmless()
    {
        var panel = Flex();

        Layout(panel, width: 200, height: 50);

        Assert.Equal(200, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void Changing_A_Grow_Factor_Changes_The_Split()
    {
        var first = Child(10, grow: 1);
        var second = Child(10, grow: 1);
        var panel = Flex(first, second);
        Layout(panel, width: 200, height: 50);

        Ex097_FlexLayoutEngine.SetGrow(second, 3);
        panel.InvalidateMeasure();
        Layout(panel, width: 200, height: 50);

        Assert.Equal(50, first.ActualWidth, 1);
        Assert.Equal(150, second.ActualWidth, 1);
    }
}
