using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex086_ControlThemeOverrideTests
{
    // The themed host and a bare Button side by side in one window, so scoping is
    // asserted against a control that is definitely styled but definitely not
    // inside the override.
    private static (Button Themed, Button Outside) Shown()
    {
        var host = Ex086_ControlThemeOverride.BuildThemedHost();
        var outside = new Button { Content = "Outside" };
        var root = new StackPanel { Children = { host, outside } };

        ViewHarness.ShowWindow(root, 300, 200);
        Dispatcher.UIThread.RunJobs();

        var themed = host.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "Themed");
        return (themed, outside);
    }

    // Structural, and it names the mechanism rather than an outcome: a theme
    // written from scratch would satisfy the two setter assertions below while
    // silently discarding everything else FluentTheme defines.
    [AvaloniaFact]
    public void The_Theme_Is_Based_On_The_Applications_Own_Button_Theme()
    {
        var theme = Ex086_ControlThemeOverride.BuildTheme();

        Assert.Equal(typeof(Button), theme.TargetType);
        Assert.NotNull(theme.BasedOn);

        Application.Current!.TryGetResource(typeof(Button), null, out var fluent);
        Assert.Same(fluent, theme.BasedOn);
    }

    [AvaloniaFact]
    public void The_Overridden_Setters_Reach_A_Button_Inside_The_Host()
    {
        var (themed, _) = Shown();

        Assert.Equal(Ex086_ControlThemeOverride.OverriddenCornerRadius, themed.CornerRadius);
        Assert.Equal(Ex086_ControlThemeOverride.OverriddenForeground, themed.Foreground);
    }

    // Scoping: a resource keyed by type reaches its own host's subtree and nothing
    // else. An answer that reaches for Application.Styles instead would change
    // this button too and fail here.
    [AvaloniaFact]
    public void A_Button_Outside_The_Host_Keeps_The_Stock_Appearance()
    {
        var (themed, outside) = Shown();

        Assert.NotEqual(themed.CornerRadius, outside.CornerRadius);
        Assert.NotEqual(themed.Foreground, outside.Foreground);
    }

    // Extending rather than replacing: the button still has its template and the
    // content presenter that renders its content, which is what BasedOn preserves
    // and what a from-scratch theme would have to rebuild by hand.
    [AvaloniaFact]
    public void The_Themed_Button_Is_Still_A_Working_Templated_Button()
    {
        var (themed, _) = Shown();

        Assert.NotNull(themed.Template);
        Assert.NotEmpty(themed.GetVisualDescendants().OfType<ContentPresenter>());
        Assert.Equal("Inside", themed.Content);
    }

    [AvaloniaFact]
    public void The_Theme_Overrides_Exactly_The_Two_Documented_Properties()
    {
        var theme = Ex086_ControlThemeOverride.BuildTheme();

        var overridden = theme.Setters
            .OfType<Setter>()
            .Select(setter => setter.Property)
            .ToList();

        Assert.Equal(2, overridden.Count);
        Assert.Contains(Button.CornerRadiusProperty, overridden);
        Assert.Contains(Button.ForegroundProperty, overridden);
    }
}
