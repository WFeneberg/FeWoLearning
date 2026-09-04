using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex028_StyleSelectorsTests
{
    private static Ex028_StyleSelectors Show() =>
        ViewHarness.Show(new Ex028_StyleSelectors(), 300, 200);

    private static bool HasFontSizeRule(Ex028_StyleSelectors view, string selectorFragment, double fontSize) =>
        view.Styles.OfType<Style>().Any(style =>
            style.Selector != null &&
            style.Selector.ToString()!.Contains(selectorFragment) &&
            style.Setters.OfType<Setter>().Any(setter =>
                setter.Property == TextBlock.FontSizeProperty &&
                setter.Value is double value &&
                value == fontSize));

    // Mechanism check: a local FontSize setter on each TextBlock produces the
    // same rendered numbers with NO Style object at all - UserControl.Styles
    // stays empty. This reaches into the Styles collection itself and would
    // fail against that cheat before ever looking at a rendered value.
    [AvaloniaFact]
    public void UserControl_Declares_A_Type_Rule_And_A_More_Specific_Descendant_Rule()
    {
        var view = Show();

        Assert.True(HasFontSizeRule(view, "TextBlock", 21),
            "expected a Style selecting TextBlock with FontSize 21");
        Assert.True(HasFontSizeRule(view, "TextBlock.tag", 33),
            "expected a Style selecting ...TextBlock.tag with FontSize 33");
    }

    [AvaloniaFact]
    public void Plain_TextBlock_Gets_The_Type_Rules_FontSize()
    {
        var view = Show();
        var plain = view.FindControl<TextBlock>("PlainText")!;

        Assert.Equal(21, plain.FontSize);
    }

    // The more specific selector (descendant combinator + class) beats the
    // plain type selector, even though both match this element.
    [AvaloniaFact]
    public void Tagged_TextBlock_Gets_The_More_Specific_Rules_FontSize()
    {
        var view = Show();
        var tagged = view.FindControl<TextBlock>("TaggedText")!;

        Assert.Contains("tag", tagged.Classes);
        Assert.Equal(33, tagged.FontSize);
    }
}
