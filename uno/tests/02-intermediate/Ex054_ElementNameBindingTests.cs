using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex054_ElementNameBindingTests : UnoTestContext
{
    private static Ex054_ElementNameBinding Control() => Layout(new Ex054_ElementNameBinding());

    /// <summary>
    /// The markup's root panel. Named separately so the failure says what is missing rather
    /// than "value is null".
    /// </summary>
    private static StackPanel FindRoot(Ex054_ElementNameBinding control)
    {
        var root = control.FindName("Root");
        Assert.True(root is not null, "the markup declares no element named Root");

        return Assert.IsType<StackPanel>(root);
    }

    [Fact]
    public void The_Mirror_Shows_The_Source_Text()
    {
        var control = Control();

        Assert.Equal("the source", FindDescendant<TextBlock>(control, "Mirror").Text);
    }

    [Fact]
    public void The_Mirror_Follows_The_Source()
    {
        var control = Control();

        FindDescendant<TextBlock>(control, "Source").Text = "changed";

        Assert.Equal("changed", FindDescendant<TextBlock>(control, "Mirror").Text);
    }

    [Fact]
    public void A_Forward_Reference_Resolves()
    {
        var control = Control();

        // "Sized" names "Dial" before "Dial" appears in the document. Name scopes are
        // resolved once the scope is complete, not line by line.
        Assert.Equal(40, FindDescendant<Border>(control, "Sized").ActualWidth, 1);
    }

    [Fact]
    public void The_Bound_Width_Follows_The_Slider()
    {
        var control = Control();

        FindDescendant<Slider>(control, "Dial").Value = 120;
        Layout(control);

        Assert.Equal(120, FindDescendant<Border>(control, "Sized").ActualWidth, 1);
    }

    [Fact]
    public void FindName_Is_The_Same_Lookup()
    {
        var control = Control();

        // The binding resolved "Source" through the same name scope FindName reads.
        Assert.Same(FindDescendant<TextBlock>(control, "Source"), control.FindName("Source"));
    }

    [Fact]
    public void The_Markup_Builds_The_Named_Root()
    {
        var control = Control();

        var root = FindRoot(control);

        Assert.Equal(4, root.Children.Count);
    }
}
