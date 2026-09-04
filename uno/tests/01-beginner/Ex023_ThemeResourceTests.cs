using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex023_ThemeResourceTests : UnoTestContext
{
    private static Color BackgroundOf(Ex023_ThemeResource control) =>
        ((SolidColorBrush)FindDescendant<Border>(control, "Card").Background).Color;

    [Fact]
    public void Declares_Both_Theme_Dictionaries()
    {
        var control = new Ex023_ThemeResource();

        // The keys are part of the contract - a dictionary keyed "dark" is never consulted.
        Assert.True(
            control.Resources.ThemeDictionaries.ContainsKey("Light"),
            "no theme dictionary keyed \"Light\" - the key is a fixed name, not a label");
        Assert.True(
            control.Resources.ThemeDictionaries.ContainsKey("Dark"),
            "no theme dictionary keyed \"Dark\"");
    }

    [Fact]
    public void Starts_On_The_Light_Brush()
    {
        var control = Layout(new Ex023_ThemeResource());

        Assert.Equal(Colors.White, BackgroundOf(control));
    }

    [Fact]
    public void Switching_To_Dark_Re_Resolves_The_Brush()
    {
        var control = Layout(new Ex023_ThemeResource());

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);

        // This is the whole difference to {StaticResource}: the reference survived the
        // build and was asked again.
        Assert.Equal(Colors.Black, BackgroundOf(control));
    }

    [Fact]
    public void Switching_Back_Returns_To_The_Light_Brush()
    {
        var control = Layout(new Ex023_ThemeResource());

        control.ApplyTheme(ElementTheme.Dark);
        Layout(control);
        control.ApplyTheme(ElementTheme.Light);
        Layout(control);

        Assert.Equal(Colors.White, BackgroundOf(control));
    }

    [Fact]
    public void The_Theme_Is_Recorded_On_The_Control_Itself()
    {
        var control = Layout(new Ex023_ThemeResource());

        control.ApplyTheme(ElementTheme.Dark);

        // RequestedTheme on the element, not Application.RequestedTheme: one subtree can
        // be dark inside an otherwise light app, which is how a preview pane is built.
        Assert.Equal(ElementTheme.Dark, control.RequestedTheme);
    }

    [Fact]
    public void Two_Controls_Can_Be_On_Different_Themes()
    {
        var light = Layout(new Ex023_ThemeResource());
        var dark = Layout(new Ex023_ThemeResource());

        dark.ApplyTheme(ElementTheme.Dark);
        Layout(dark);

        Assert.Equal(Colors.White, BackgroundOf(light));
        Assert.Equal(Colors.Black, BackgroundOf(dark));
    }

    [Fact]
    public void The_Card_Has_The_Size_The_Markup_Gave_It()
    {
        var control = Layout(new Ex023_ThemeResource());

        var card = FindDescendant<Border>(control, "Card");

        Assert.Equal(40, card.ActualWidth, 1);
        Assert.Equal(20, card.ActualHeight, 1);
    }
}
