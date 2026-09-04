using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex017_VisibilityCollapsedTests : UnoTestContext
{
    private static Border Box() => new() { Width = 30, Height = 20 };

    private static StackPanel Row(out Border middle)
    {
        middle = Box();
        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(Box());
        panel.Children.Add(middle);
        panel.Children.Add(Box());
        return panel;
    }

    [Fact]
    public void Collapsing_Uses_Visibility_And_Leaves_Opacity_Alone()
    {
        var box = Box();

        Ex017_VisibilityCollapsed.Hide(box, keepSpace: false);

        Assert.Equal(Visibility.Collapsed, box.Visibility);
        Assert.Equal(1, box.Opacity);
    }

    [Fact]
    public void Keeping_Space_Uses_Opacity_And_Leaves_Visibility_Alone()
    {
        var box = Box();

        Ex017_VisibilityCollapsed.Hide(box, keepSpace: true);

        Assert.Equal(0, box.Opacity);
        Assert.Equal(Visibility.Visible, box.Visibility);
    }

    [Fact]
    public void A_Collapsed_Element_Asks_For_Nothing()
    {
        var panel = Row(out var middle);
        Ex017_VisibilityCollapsed.Hide(middle, keepSpace: false);

        Layout(panel);

        Assert.Equal(0, middle.DesiredSize.Height, 1);
    }

    [Fact]
    public void A_Transparent_Element_Is_Still_Measured()
    {
        var panel = Row(out var middle);
        Ex017_VisibilityCollapsed.Hide(middle, keepSpace: true);

        Layout(panel);

        Assert.Equal(20, middle.DesiredSize.Height, 1);
    }

    [Fact]
    public void Collapsing_Takes_The_Gap_With_It()
    {
        var panel = Row(out var middle);
        Ex017_VisibilityCollapsed.Hide(middle, keepSpace: false);

        Layout(panel);

        // Two visible children and one gap: 20 + 10 + 20. The StackPanel does not leave
        // a hole where the collapsed child was, nor a double gap around it.
        Assert.Equal(50, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Going_Transparent_Keeps_Everything_Where_It_Was()
    {
        var panel = Row(out var middle);
        Ex017_VisibilityCollapsed.Hide(middle, keepSpace: true);

        Layout(panel);

        Assert.Equal(80, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void The_Choice_Only_Shows_Up_In_The_Layout()
    {
        var collapsedPanel = Row(out var collapsed);
        Ex017_VisibilityCollapsed.Hide(collapsed, keepSpace: false);
        var transparentPanel = Row(out var transparent);
        Ex017_VisibilityCollapsed.Hide(transparent, keepSpace: true);

        Layout(collapsedPanel);
        Layout(transparentPanel);

        // Both are equally invisible on screen; only one of them still costs 30 pixels.
        Assert.Equal(30, transparentPanel.DesiredSize.Height - collapsedPanel.DesiredSize.Height, 1);
    }
}
