using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex009_XamlUserControlTests : UnoTestContext
{
    [Fact]
    public void Markup_Is_Actually_Run()
    {
        var control = new Ex009_XamlUserControl();

        // Compiled markup that InitializeComponent never executes leaves Content null.
        Assert.NotNull(control.Content);
    }

    [Fact]
    public void Names_The_Root_Panel()
    {
        var control = new Ex009_XamlUserControl();

        var root = Assert.IsType<StackPanel>(control.FindName("Root"));

        Assert.Equal(2, root.Children.Count);
    }

    [Fact]
    public void Reads_The_Caption_The_Markup_Declared()
    {
        Assert.Equal("Hello Uno", new Ex009_XamlUserControl().CaptionText);
    }

    [Fact]
    public void Writing_The_Caption_Reaches_The_Named_TextBlock()
    {
        var control = new Ex009_XamlUserControl();

        control.CaptionText = "Guten Tag";

        // Asserted through the tree, not through the property that was just written.
        Assert.Equal("Guten Tag", FindDescendant<TextBlock>(control, "Caption").Text);
    }

    [Fact]
    public void The_Box_Has_The_Size_The_Markup_Gave_It()
    {
        var control = Layout(new Ex009_XamlUserControl());

        var box = FindDescendant<Border>(control, "Box");

        Assert.Equal(30, box.ActualWidth, 1);
        Assert.Equal(40, box.ActualHeight, 1);
    }

    [Fact]
    public void Stacks_The_Caption_Above_The_Box()
    {
        var control = Layout(new Ex009_XamlUserControl());

        // A StackPanel adds its children's heights up; a Grid would overlap them at 40.
        Assert.True(
            control.DesiredSize.Height > 40,
            $"expected caption plus box, got {control.DesiredSize.Height}");
    }
}
