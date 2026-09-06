using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex086_
public static class Ex086_ControlThemeOverride
{
    /// <summary>Given. Do not change.</summary>
    public static CornerRadius OverriddenCornerRadius { get; } = new(12);

    /// <summary>Given. Do not change.</summary>
    public static IBrush OverriddenForeground { get; } = Brushes.OrangeRed;

    public static ControlTheme BuildTheme()
    {
        // The application is the only host that answers for typeof(Button): the
        // button's own Theme property is null and neither it nor its window has the
        // resource.
        ControlTheme? fluent = null;

        if (Application.Current?.TryGetResource(typeof(Button), null, out var found) == true)
        {
            fluent = found as ControlTheme;
        }

        return new ControlTheme(typeof(Button))
        {
            BasedOn = fluent,
            Setters =
            {
                new Setter(Button.CornerRadiusProperty, OverriddenCornerRadius),
                new Setter(Button.ForegroundProperty, OverriddenForeground),
            },
        };
    }

    public static Control BuildThemedHost()
    {
        var host = new StackPanel();

        // Keyed by the type it applies to, which is what makes it implicit, and put
        // in place before the host is ever shown.
        host.Resources[typeof(Button)] = BuildTheme();
        host.Children.Add(new Button { Name = "Themed", Content = "Inside" });

        return host;
    }
}
