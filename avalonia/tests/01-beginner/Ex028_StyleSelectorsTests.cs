using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex028_StyleSelectorsTests
{
    private static Ex028_StyleSelectors Show() =>
        ViewHarness.Show(new Ex028_StyleSelectors(), 300, 200);

    // Every Style rule declared anywhere in the tree, not just the root
    // UserControl's own Styles collection - a rule scoped to an inner
    // element's Styles (e.g. the StackPanel's) is just as valid Avalonia as
    // one declared on the root, and must not be rejected.
    private static IEnumerable<Style> AllStyles(Visual root) =>
        root.GetSelfAndVisualDescendants()
            .OfType<StyledElement>()
            .SelectMany(e => e.Styles)
            .OfType<Style>();

    private static bool HasFontSizeRule(Visual root, Func<string, bool> selectorPredicate, double fontSize) =>
        AllStyles(root).Any(style =>
            style.Selector != null &&
            selectorPredicate(style.Selector.ToString()!) &&
            style.Setters.OfType<Setter>().Any(setter =>
                setter.Property == TextBlock.FontSizeProperty &&
                setter.Value is double value &&
                value == fontSize));

    // Structural check, scoped to wherever the rules actually live: a local
    // FontSize setter on each TextBlock produces the same rendered numbers
    // with NO Style object anywhere in the tree - this walk stays empty of
    // matches against that cheat before ever looking at a rendered value.
    //
    // The second assertion also drills the "descendant" half of this
    // exercise's own concept: a bare class selector ("TextBlock.tag", no
    // combinator) still outranks the plain type selector and would satisfy
    // the FIRST assertion's fragment check just as well, so it additionally
    // requires the selector text carry more than just "TextBlock.tag" -
    // i.e. an actual combinator, without pinning its exact spelling.
    [AvaloniaFact]
    public void UserControl_Declares_A_Type_Rule_And_A_More_Specific_Descendant_Rule()
    {
        var view = Show();

        Assert.True(HasFontSizeRule(view, s => s.Contains("TextBlock"), 21),
            "expected a Style selecting TextBlock with FontSize 21");
        Assert.True(
            HasFontSizeRule(view, s => s.Contains("TextBlock.tag") && s.Trim() != "TextBlock.tag", 33),
            "expected a Style selecting ...TextBlock.tag through a descendant " +
            "combinator (not a bare class selector) with FontSize 33");
    }

    // The FontSize must come from a Style, not a local value that merely
    // renders the same number - a StyledProperty's BindingPriority tells
    // them apart regardless of which element in the tree owns the Style
    // that set it.
    [AvaloniaFact]
    public void Plain_TextBlock_Gets_The_Type_Rules_FontSize_From_A_Style()
    {
        var view = Show();
        var plain = view.FindControl<TextBlock>("PlainText")!;

        Assert.Equal(21, plain.FontSize);
        Assert.NotEqual(BindingPriority.LocalValue, plain.GetDiagnostic(TextBlock.FontSizeProperty).Priority);
    }

    // The more specific selector (descendant combinator + class) beats the
    // plain type selector, even though both match this element.
    [AvaloniaFact]
    public void Tagged_TextBlock_Gets_The_More_Specific_Rules_FontSize_From_A_Style()
    {
        var view = Show();
        var tagged = view.FindControl<TextBlock>("TaggedText")!;

        Assert.Contains("tag", tagged.Classes);
        Assert.Equal(33, tagged.FontSize);
        Assert.NotEqual(BindingPriority.LocalValue, tagged.GetDiagnostic(TextBlock.FontSizeProperty).Priority);
    }
}
