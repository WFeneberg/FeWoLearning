using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 088 - ThemeVariantSwitching (advanced).
/// Goal:   Define one brush twice - once for light, once for dark - and let a
///         control follow the variant as it changes at run time, without any code
///         reassigning anything.
/// Drills: ThemeVariantScope, RequestedThemeVariant versus ActualThemeVariant,
///         ResourceDictionary.ThemeDictionaries, DynamicResource across a variant
///         switch, nested scopes.
/// Passes: dotnet test --filter FullyQualifiedName~Ex088_
///
/// RequestedThemeVariant is what you SET; ActualThemeVariant is what a control
/// ends up with, resolved from the nearest scope that requested something. Setting
/// the requested one to Default hands the decision back up the tree, which is why
/// both exist.
///
/// The consumer must bind with a DynamicResource, and that is not a style
/// preference: measured, TryGetResource on a control inside the scope does NOT
/// find theme-dictionary entries at all, while the same key read through a
/// DynamicResource binding resolved correctly and re-resolved on every switch. See
/// ex087 for the general form of that finding.
public static class Ex088_ThemeVariantSwitching
{
    /// <summary>Given. Do not change. The key defined once per variant.</summary>
    public const string AccentKey = "Accent";

    /// <summary>Given. Do not change. What AccentKey means under the light variant.</summary>
    public static IBrush LightAccent { get; } = Brushes.MidnightBlue;

    /// <summary>Given. Do not change. What AccentKey means under the dark variant.</summary>
    public static IBrush DarkAccent { get; } = Brushes.Gold;

    /// <summary>
    /// A ThemeVariantScope that starts out requesting the LIGHT variant, carries a
    /// resource dictionary defining AccentKey for both variants, and whose content
    /// is a PANEL holding a Border named "Accented" whose Background follows that
    /// key. A panel rather than the Border directly, so the nested scope below has
    /// somewhere to go.
    ///
    /// Put the two definitions in the dictionary's ThemeDictionaries, one under the
    /// light variant and one under the dark - not as two differently named keys,
    /// which would defeat the whole mechanism.
    /// </summary>
    public static ThemeVariantScope BuildScope() =>
        throw new NotImplementedException(
            "TODO: Ex088 - a ThemeVariantScope requesting ThemeVariant.Light, with a " +
            "ResourceDictionary merged in whose ThemeDictionaries define AccentKey " +
            "as LightAccent and DarkAccent, whose Child is a panel holding a Border " +
            "named Accented with its Background bound to AccentKey by a " +
            "DynamicResource");

    /// <summary>
    /// A scope nested inside <paramref name="outer"/>'s content panel that requests
    /// the DARK variant and holds its own Border named "Inner", bound to the same
    /// key.
    ///
    /// It does not define the key again: a nested scope inherits the resources and
    /// changes only which variant is asked for, which is what makes a "dark card on
    /// a light page" cheap.
    /// </summary>
    public static ThemeVariantScope BuildNestedDarkScope(ThemeVariantScope outer) =>
        throw new NotImplementedException(
            "TODO: Ex088 - a ThemeVariantScope requesting ThemeVariant.Dark, holding " +
            "a Border named Inner bound to AccentKey the same way, added to outer's " +
            "content panel so it inherits the dictionary");
}
