using FeWoLearning.Uno.Exercises.Beginner;
using FeWoLearning.Uno.Support;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex018_DataContextInheritanceTests : UnoTestContext
{
    private static (Border Root, CaptionSource Outer, CaptionSource Inner) Tree()
    {
        var outer = new CaptionSource { Caption = "outer" };
        var inner = new CaptionSource { Caption = "inner" };
        return (Ex018_DataContextInheritance.CreateNestedLabels(outer, inner), outer, inner);
    }

    [Fact]
    public void The_Outer_Label_Reads_The_Inherited_Context()
    {
        var (root, _, _) = Tree();

        Assert.Equal("outer", FindDescendant<TextBlock>(root, "Outer").Text);
    }

    [Fact]
    public void The_Inner_Label_Reads_Its_Own_Context()
    {
        var (root, _, _) = Tree();

        Assert.Equal("inner", FindDescendant<TextBlock>(root, "Inner").Text);
    }

    [Fact]
    public void The_Context_Reaches_Elements_That_Never_Set_It()
    {
        var (root, outer, _) = Tree();

        var panel = FindDescendant<StackPanel>(root);

        // DataContext is an inherited property: the panel in between was never assigned
        // one, and still has it.
        Assert.Same(outer, panel.DataContext);
    }

    [Fact]
    public void Changing_The_Outer_Source_Moves_Only_The_Outer_Label()
    {
        var (root, outer, _) = Tree();

        outer.Caption = "changed";

        Assert.Equal("changed", FindDescendant<TextBlock>(root, "Outer").Text);
        Assert.Equal("inner", FindDescendant<TextBlock>(root, "Inner").Text);
    }

    [Fact]
    public void Changing_The_Inner_Source_Moves_Only_The_Inner_Label()
    {
        var (root, _, inner) = Tree();

        inner.Caption = "changed";

        Assert.Equal("outer", FindDescendant<TextBlock>(root, "Outer").Text);
        Assert.Equal("changed", FindDescendant<TextBlock>(root, "Inner").Text);
    }

    [Fact]
    public void Replacing_The_Root_Context_Re_Evaluates_The_Inherited_Binding()
    {
        var (root, _, _) = Tree();

        root.DataContext = new CaptionSource { Caption = "replaced" };

        // The binding tracks the context, not the object that happened to be there when
        // it was attached - which is what makes a template reusable per item.
        Assert.Equal("replaced", FindDescendant<TextBlock>(root, "Outer").Text);
        Assert.Equal("inner", FindDescendant<TextBlock>(root, "Inner").Text);
    }
}
