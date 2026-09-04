using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex012_StackPanelSpacingTests : UnoTestContext
{
    private static Border Box(double width, double height) => new() { Width = width, Height = height };

    [Fact]
    public void Stacks_Vertically_With_Gaps_Between_Children()
    {
        var panel = Layout(Ex012_StackPanelSpacing.Create(
            Orientation.Vertical,
            8,
            Box(30, 20),
            Box(30, 20),
            Box(30, 20)));

        // Three children, two gaps: 60 + 16. Not 60 + 24.
        Assert.Equal(76, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Takes_The_Widest_Child_Across_The_Stacking_Axis()
    {
        var panel = Layout(Ex012_StackPanelSpacing.Create(
            Orientation.Vertical,
            8,
            Box(30, 20),
            Box(70, 20)));

        // Spacing applies along the stack only; the cross axis is a maximum, not a sum.
        Assert.Equal(70, panel.DesiredSize.Width, 1);
    }

    [Fact]
    public void Stacks_Horizontally_When_Asked_To()
    {
        var panel = Layout(Ex012_StackPanelSpacing.Create(
            Orientation.Horizontal,
            8,
            Box(30, 20),
            Box(30, 20),
            Box(30, 20)));

        Assert.Equal(Orientation.Horizontal, panel.Orientation);
        Assert.Equal(106, panel.DesiredSize.Width, 1);
        Assert.Equal(20, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void A_Single_Child_Gets_No_Spacing_At_All()
    {
        var panel = Layout(Ex012_StackPanelSpacing.Create(Orientation.Vertical, 8, Box(30, 20)));

        Assert.Equal(20, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Positions_Each_Child_After_The_Previous_One_Plus_The_Gap()
    {
        var first = Box(30, 20);
        var second = Box(30, 20);
        var third = Box(30, 20);

        Layout(Ex012_StackPanelSpacing.Create(Orientation.Vertical, 8, first, second, third));

        Assert.Equal(0, Offset(first).Y, 1);
        Assert.Equal(28, Offset(second).Y, 1);
        Assert.Equal(56, Offset(third).Y, 1);
    }

    [Fact]
    public void Keeps_Every_Child_In_The_Order_It_Was_Given()
    {
        var first = Box(30, 20);
        var second = Box(30, 20);

        var panel = Ex012_StackPanelSpacing.Create(Orientation.Vertical, 8, first, second);

        Assert.Same(first, panel.Children[0]);
        Assert.Same(second, panel.Children[1]);
    }
}
