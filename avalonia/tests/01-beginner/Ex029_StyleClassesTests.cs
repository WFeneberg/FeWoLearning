using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex029_StyleClassesTests
{
    private static Ex029_StyleClasses Show() => ViewHarness.Show(new Ex029_StyleClasses(), 300, 160);

    private static void Click(Visual target)
    {
        var top = TopLevel.GetTopLevel(target)!;
        var p = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), top)!.Value;
        top.MouseDown(p, MouseButton.Left);
        top.MouseUp(p, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    // Every Style rule declared anywhere in the tree, not just the root
    // UserControl's own Styles collection - a rule scoped to an inner
    // element's Styles is just as valid Avalonia as one declared on the root.
    private static IEnumerable<Style> AllStyles(Visual root) =>
        root.GetSelfAndVisualDescendants()
            .OfType<StyledElement>()
            .SelectMany(e => e.Styles)
            .OfType<Style>();

    // Structural check: a code-behind handler that pokes FontSize directly,
    // without ever touching Classes or declaring a Style, leaves this walk
    // empty of matches - it can never be satisfied that way.
    [AvaloniaFact]
    public void UserControl_Declares_A_Class_Selector_Rule()
    {
        var view = Show();

        var hasTagRule = AllStyles(view).Any(style =>
            style.Selector != null &&
            style.Selector.ToString()!.Contains("TextBlock.tag") &&
            style.Setters.OfType<Setter>().Any(setter =>
                setter.Property == TextBlock.FontSizeProperty &&
                setter.Value is double value &&
                value == 33));

        Assert.True(hasTagRule,
            "expected a Style selecting TextBlock.tag with FontSize 33 (declared anywhere in the tree)");
    }

    [AvaloniaFact]
    public void Toggle_Starts_Unstyled_Without_The_Tag_Class()
    {
        var view = Show();
        var toggle = view.FindControl<TextBlock>("Toggle")!;

        Assert.DoesNotContain("tag", toggle.Classes);
        Assert.Equal(21, toggle.FontSize);
    }

    // The real discriminator: the toggle has to work in BOTH directions -
    // add the class and reflect the styled size, then remove it and land
    // back on the original, unstyled size. A one-shot switch or a value
    // poked directly onto FontSize (bypassing Classes) fails one of these
    // two assertion pairs. The BindingPriority check on top additionally
    // proves the value came from a Style rather than a local value that
    // merely happens to render the same number - it holds regardless of
    // which element in the tree owns the Style.
    [AvaloniaFact]
    public void Clicking_Toggles_The_Tag_Class_And_The_Styled_FontSize_Both_Ways()
    {
        var view = Show();
        var toggle = view.FindControl<TextBlock>("Toggle")!;
        var button = view.FindControl<Button>("ToggleButton")!;

        Click(button);
        Assert.Contains("tag", toggle.Classes);
        Assert.Equal(33, toggle.FontSize);
        Assert.NotEqual(BindingPriority.LocalValue, toggle.GetDiagnostic(TextBlock.FontSizeProperty).Priority);

        Click(button);
        Assert.DoesNotContain("tag", toggle.Classes);
        Assert.Equal(21, toggle.FontSize);
        Assert.NotEqual(BindingPriority.LocalValue, toggle.GetDiagnostic(TextBlock.FontSizeProperty).Priority);
    }
}
