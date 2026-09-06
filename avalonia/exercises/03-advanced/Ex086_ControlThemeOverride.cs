using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 086 - ControlThemeOverride (advanced).
/// Goal:   Change how every Button inside one part of the tree looks, without
///         rewriting its template and without touching the rest of the
///         application: build a ControlTheme BASED ON the one FluentTheme already
///         supplies, override two setters, and scope it to a host's resources.
/// Drills: ControlTheme with BasedOn, an implicit theme keyed by type in a
///         ResourceDictionary, resource scoping, Application.TryGetResource.
/// Passes: dotnet test --filter FullyQualifiedName~Ex086_
///
/// FINDING THE THEME YOU MEAN TO EXTEND IS THE AWKWARD PART, and measured, so
/// here it is. A styled Button's own Theme property reads NULL - the default
/// theme is resolved internally and never assigned there - and asking the button
/// or its window for typeof(Button) as a resource fails too. The one place it can
/// be found is the application: Application.Current.TryGetResource(typeof(Button),
/// null, out var theme) returns true and hands back the FluentTheme's ControlTheme.
///
/// Scoping is the other half. An implicit theme is a resource keyed by the TYPE it
/// applies to, so putting it in a host's Resources reaches every Button inside
/// that host and no Button outside it - which is exactly what makes this different
/// from adding a Style to Application.Styles.
///
/// One timing trap, also measured: assigning the theme resource AFTER the host is
/// already shown does not take effect. Build the host with its resources in place,
/// which is what the method below does anyway.
public static class Ex086_ControlThemeOverride
{
    /// <summary>Given. Do not change. What the override sets the corner radius to.</summary>
    public static CornerRadius OverriddenCornerRadius { get; } = new(12);

    /// <summary>Given. Do not change. What the override sets the foreground to.</summary>
    public static IBrush OverriddenForeground { get; } = Brushes.OrangeRed;

    /// <summary>
    /// A ControlTheme for Button that keeps everything FluentTheme does and changes
    /// only CornerRadius and Foreground, to the two values above.
    ///
    /// BasedOn is not decoration: a ControlTheme built without it replaces the
    /// default rather than extending it, and the test checks that BasedOn is the
    /// application's own Button theme rather than merely non-null.
    /// </summary>
    public static ControlTheme BuildTheme() =>
        throw new NotImplementedException(
            "TODO: Ex086 - a new ControlTheme(typeof(Button)) whose BasedOn is the " +
            "ControlTheme that Application.Current.TryGetResource(typeof(Button), " +
            "null, out ...) yields, with Setters for CornerRadiusProperty and " +
            "ForegroundProperty");

    /// <summary>
    /// A host carrying that theme as an implicit resource, containing exactly one
    /// Button named "Themed" with the content "Inside".
    ///
    /// Anything the test puts NEXT TO this host, rather than inside it, must keep
    /// the stock appearance - so the theme belongs in this host's own Resources,
    /// not in the window's and not in the application's.
    /// </summary>
    public static Control BuildThemedHost() =>
        throw new NotImplementedException(
            "TODO: Ex086 - a panel whose Resources[typeof(Button)] is BuildTheme(), " +
            "holding a Button named Themed with the content \"Inside\"");
}
