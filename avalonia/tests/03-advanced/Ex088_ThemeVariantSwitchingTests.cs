using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex088_ThemeVariantSwitchingTests
{
    private static ThemeVariantScope Shown()
    {
        var scope = Ex088_ThemeVariantSwitching.BuildScope();
        ViewHarness.ShowWindow(scope, 300, 200);
        Dispatcher.UIThread.RunJobs();
        return scope;
    }

    private static Border Border(ThemeVariantScope scope, string name) =>
        scope.GetVisualDescendants().OfType<Border>().Single(b => b.Name == name);

    [AvaloniaFact]
    public void The_Scope_Starts_Out_On_The_Light_Variant()
    {
        var scope = Shown();

        Assert.Equal(ThemeVariant.Light, scope.RequestedThemeVariant);
        Assert.Equal(ThemeVariant.Light, scope.ActualThemeVariant);
        Assert.Equal(Ex088_ThemeVariantSwitching.LightAccent, Border(scope, "Accented").Background);
    }

    // The whole mechanism in one assertion: nothing reassigns the brush, only the
    // variant changes, and the bound value follows. A solution that resolves the
    // brush once at construction passes the test above and fails this one.
    [AvaloniaFact]
    public void Switching_The_Variant_Moves_The_Bound_Brush()
    {
        var scope = Shown();
        var accented = Border(scope, "Accented");

        scope.RequestedThemeVariant = ThemeVariant.Dark;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(ThemeVariant.Dark, scope.ActualThemeVariant);
        Assert.Equal(Ex088_ThemeVariantSwitching.DarkAccent, accented.Background);
    }

    [AvaloniaFact]
    public void Switching_Back_Restores_The_Light_Brush()
    {
        var scope = Shown();
        var accented = Border(scope, "Accented");

        scope.RequestedThemeVariant = ThemeVariant.Dark;
        Dispatcher.UIThread.RunJobs();
        scope.RequestedThemeVariant = ThemeVariant.Light;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(Ex088_ThemeVariantSwitching.LightAccent, accented.Background);
    }

    // Two variants live at once, which is what a dark card on a light page needs.
    // It also proves the key really is one key: the inner Border resolves it
    // differently only because its nearest scope asks for something else.
    [AvaloniaFact]
    public void A_Nested_Scope_Resolves_The_Same_Key_Differently()
    {
        var outer = Shown();
        var nested = Ex088_ThemeVariantSwitching.BuildNestedDarkScope(outer);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(ThemeVariant.Light, outer.ActualThemeVariant);
        Assert.Equal(ThemeVariant.Dark, nested.ActualThemeVariant);
        Assert.Equal(Ex088_ThemeVariantSwitching.LightAccent, Border(outer, "Accented").Background);
        Assert.Equal(Ex088_ThemeVariantSwitching.DarkAccent, Border(outer, "Inner").Background);
    }

    // Requesting Default hands the decision back up the tree, which is the reason
    // ActualThemeVariant is a separate property from RequestedThemeVariant.
    [AvaloniaFact]
    public void A_Nested_Scope_Asking_For_Default_Follows_The_Outer_One()
    {
        var outer = Shown();
        var nested = Ex088_ThemeVariantSwitching.BuildNestedDarkScope(outer);
        Dispatcher.UIThread.RunJobs();

        nested.RequestedThemeVariant = ThemeVariant.Default;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(ThemeVariant.Light, nested.ActualThemeVariant);
        Assert.Equal(Ex088_ThemeVariantSwitching.LightAccent, Border(outer, "Inner").Background);
    }

    // And the nested scope follows the outer one when THAT changes, rather than
    // having latched a value.
    [AvaloniaFact]
    public void A_Defaulting_Nested_Scope_Follows_A_Later_Outer_Switch()
    {
        var outer = Shown();
        var nested = Ex088_ThemeVariantSwitching.BuildNestedDarkScope(outer);
        nested.RequestedThemeVariant = ThemeVariant.Default;
        Dispatcher.UIThread.RunJobs();

        outer.RequestedThemeVariant = ThemeVariant.Dark;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(ThemeVariant.Dark, nested.ActualThemeVariant);
        Assert.Equal(Ex088_ThemeVariantSwitching.DarkAccent, Border(outer, "Inner").Background);
    }

    // The measured trap, same shape as ex087's: the lookup does not walk up, so
    // the binding is the only route. Stated as a test so nobody "simplifies" the
    // solution into a TryGetResource call.
    [AvaloniaFact]
    public void TryGetResource_On_The_Consumer_Does_Not_Find_The_Themed_Key()
    {
        var scope = Shown();

        Assert.False(Border(scope, "Accented").TryGetResource(
            Ex088_ThemeVariantSwitching.AccentKey, scope.ActualThemeVariant, out var value));
        Assert.Null(value);
    }
}
