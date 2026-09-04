using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex030_PseudoClassesTests
{
    private static (Ex030_PseudoClasses View, Ex030_PseudoClassesViewModel Vm) Arrange()
    {
        var vm = new Ex030_PseudoClassesViewModel();
        var view = ViewHarness.Show(new Ex030_PseudoClasses { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    // Every Style rule declared anywhere in the tree, not just the root
    // UserControl's own Styles collection - a rule scoped to an inner
    // element's Styles is just as valid Avalonia as one declared on the root.
    private static IEnumerable<Style> AllStyles(Visual root) =>
        root.GetSelfAndVisualDescendants()
            .OfType<StyledElement>()
            .SelectMany(e => e.Styles)
            .OfType<Style>();

    private static bool HasOpacityRule(Visual root, string selectorFragment, double opacity) =>
        AllStyles(root).Any(style =>
            style.Selector != null &&
            style.Selector.ToString()!.Contains(selectorFragment) &&
            style.Setters.OfType<Setter>().Any(setter =>
                setter.Property == Visual.OpacityProperty &&
                setter.Value is double value &&
                value == opacity));

    // Structural check: a PointerEntered/PointerExited (or CanExecute-driven)
    // code-behind handler that pokes Opacity directly, with no Style at all,
    // leaves this walk empty of matches - it can never be satisfied that way.
    [AvaloniaFact]
    public void UserControl_Declares_PointerOver_And_Disabled_Rules()
    {
        var (view, _) = Arrange();

        Assert.True(HasOpacityRule(view, ":pointerover", 0.5),
            "expected a Style selecting Button:pointerover with Opacity 0.5 (declared anywhere in the tree)");
        Assert.True(HasOpacityRule(view, ":disabled", 0.3),
            "expected a Style selecting Button:disabled with Opacity 0.3 (declared anywhere in the tree)");
    }

    [AvaloniaFact]
    public void Button_Starts_Unstyled_Neither_Hovered_Nor_Disabled()
    {
        var (view, _) = Arrange();
        var button = view.FindControl<Button>("ActionButton")!;

        Assert.Equal(1, button.Opacity);
        Assert.False(button.IsPointerOver);
        Assert.DoesNotContain(":pointerover", button.Classes);
        Assert.DoesNotContain(":disabled", button.Classes);
    }

    // The real discriminator against a code-behind PointerEntered/PointerExited
    // handler: it asserts the pseudo-class membership itself, not only its
    // Opacity consequence, drives both directions (enter then leave), and the
    // BindingPriority check proves the Opacity came from a Style rather than
    // a code-behind value that merely renders the same number - it holds
    // regardless of which element in the tree owns the Style.
    [AvaloniaFact]
    public void Hovering_Adds_PointerOver_And_Leaving_Removes_It_Both_Ways()
    {
        var (view, _) = Arrange();
        var button = view.FindControl<Button>("ActionButton")!;
        var top = TopLevel.GetTopLevel(view)!;

        var over = button.TranslatePoint(
            new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), top)!.Value;
        top.MouseMove(over);
        Dispatcher.UIThread.RunJobs();

        Assert.True(button.IsPointerOver);
        Assert.Contains(":pointerover", button.Classes);
        Assert.Equal(0.5, button.Opacity);
        Assert.NotEqual(BindingPriority.LocalValue, button.GetDiagnostic(Visual.OpacityProperty).Priority);

        // Move well away from the button, still inside the window.
        top.MouseMove(new Point(290, 190));
        Dispatcher.UIThread.RunJobs();

        Assert.False(button.IsPointerOver);
        Assert.DoesNotContain(":pointerover", button.Classes);
        Assert.Equal(1, button.Opacity);
    }

    // Same shape for :disabled, driven the honest way (CanExecute going
    // false), not by setting IsEnabled directly - see Ex017 for why that
    // distinction matters. Both directions again: disable, then re-enable.
    [AvaloniaFact]
    public void Disabling_The_Command_Adds_Disabled_And_ReEnabling_Removes_It_Both_Ways()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("ActionButton")!;

        vm.CanRun = false;
        Dispatcher.UIThread.RunJobs();

        Assert.False(button.IsEffectivelyEnabled);
        Assert.Contains(":disabled", button.Classes);
        Assert.Equal(0.3, button.Opacity);
        Assert.NotEqual(BindingPriority.LocalValue, button.GetDiagnostic(Visual.OpacityProperty).Priority);

        vm.CanRun = true;
        Dispatcher.UIThread.RunJobs();

        Assert.True(button.IsEffectivelyEnabled);
        Assert.DoesNotContain(":disabled", button.Classes);
        Assert.Equal(1, button.Opacity);
    }
}
