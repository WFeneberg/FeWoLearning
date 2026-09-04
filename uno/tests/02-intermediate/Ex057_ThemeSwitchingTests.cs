using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex057_ThemeSwitchingTests : UnoTestContext
{
    private static Ex057_ThemeSwitching Control() => Layout(new Ex057_ThemeSwitching());

    private static Color BackgroundOf(Ex057_ThemeSwitching control, string name) =>
        ((SolidColorBrush)FindDescendant<Border>(control, name).Background).Color;

    [Fact]
    public void Starts_On_The_Light_Values()
    {
        var control = Control();

        Assert.Equal(Colors.White, BackgroundOf(control, "Live"));
        Assert.Equal(40, FindDescendant<Border>(control, "Frozen").ActualWidth, 1);
    }

    [Fact]
    public void A_Theme_Resource_Follows_The_Switch()
    {
        var control = Control();

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);

        Assert.Equal(Colors.Black, BackgroundOf(control, "Live"));
    }

    [Fact]
    public void A_Static_Resource_Does_Not()
    {
        var control = Control();

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);

        // Resolved once while the tree was built, with no live link left behind. The Dark
        // dictionary says 80 and this element will never hear about it.
        Assert.Equal(40, FindDescendant<Border>(control, "Frozen").ActualWidth, 1);
    }

    [Fact]
    public void The_Theme_Reaches_A_Nested_Element()
    {
        var control = Control();

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);

        Assert.Equal(Colors.Black, BackgroundOf(control, "Nested"));
    }

    [Fact]
    public void An_Inner_Scope_Can_Differ_From_The_Outer_One()
    {
        var control = Control();

        control.ApplyThemeToWrapperOnly(ElementTheme.Dark);
        Layout(control);

        // One subtree dark inside a light control: RequestedTheme is a FrameworkElement
        // property, so every element can start a new scope.
        Assert.Equal(Colors.Black, BackgroundOf(control, "Nested"));
        Assert.Equal(Colors.White, BackgroundOf(control, "Live"));
    }

    [Fact]
    public void Switching_Back_Restores_The_Light_Values()
    {
        var control = Control();

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);
        control.ApplyTheme(ElementTheme.Light);
        Layout(control);

        Assert.Equal(Colors.White, BackgroundOf(control, "Live"));
    }

    [Fact]
    public void The_Wrapper_Records_Its_Own_Theme()
    {
        var control = Control();

        control.ApplyThemeToWrapperOnly(ElementTheme.Dark);

        Assert.Equal(ElementTheme.Dark, FindDescendant<Border>(control, "Wrapper").RequestedTheme);
        Assert.Equal(ElementTheme.Default, control.RequestedTheme);
    }
}
