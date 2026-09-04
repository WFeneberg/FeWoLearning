using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex022_StaticResourceTests : UnoTestContext
{
    private static Ex022_StaticResource Control() => Layout(new Ex022_StaticResource());

    [Fact]
    public void Declares_The_Value_On_Its_Own_Resources()
    {
        var control = new Ex022_StaticResource();

        Assert.Equal(120d, control.Resources["CardWidth"]);
    }

    [Fact]
    public void An_Element_In_The_Outer_Scope_Gets_The_Outer_Value()
    {
        var control = Control();

        Assert.Equal(120, FindDescendant<Border>(control, "Outer").ActualWidth, 1);
    }

    [Fact]
    public void The_Inner_Scope_Shadows_The_Key_For_Itself()
    {
        var control = Control();

        // The element that declares the shadowing dictionary can see it too: the walk
        // starts at the element itself, not at its parent.
        Assert.Equal(60, FindDescendant<Border>(control, "Shadowing").ActualWidth, 1);
    }

    [Fact]
    public void The_Inner_Scope_Shadows_The_Key_For_Its_Children()
    {
        var control = Control();

        Assert.Equal(60, FindDescendant<Border>(control, "Inner").ActualWidth, 1);
    }

    [Fact]
    public void One_Key_Two_Answers_In_One_Tree()
    {
        var control = Control();

        var outer = FindDescendant<Border>(control, "Outer");
        var inner = FindDescendant<Border>(control, "Inner");

        Assert.NotEqual(outer.ActualWidth, inner.ActualWidth);
    }

    [Fact]
    public void The_Inner_Dictionary_Holds_Only_Its_Own_Override()
    {
        var control = Control();

        var shadowing = FindDescendant<Border>(control, "Shadowing");

        Assert.Equal(60d, shadowing.Resources["CardWidth"]);
    }

    [Fact]
    public void Changing_The_Dictionary_Afterwards_Does_Not_Move_Anything()
    {
        var control = Control();

        control.Resources["CardWidth"] = 999d;
        Layout(control);

        // {StaticResource} resolved while the tree was built and left no live link behind.
        // Wanting one is what {ThemeResource} and bindings are for.
        Assert.Equal(120, FindDescendant<Border>(control, "Outer").ActualWidth, 1);
    }
}
