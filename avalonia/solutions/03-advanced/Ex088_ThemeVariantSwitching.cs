using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex088_
public static class Ex088_ThemeVariantSwitching
{
    /// <summary>Given. Do not change.</summary>
    public const string AccentKey = "Accent";

    /// <summary>Given. Do not change.</summary>
    public static IBrush LightAccent { get; } = Brushes.MidnightBlue;

    /// <summary>Given. Do not change.</summary>
    public static IBrush DarkAccent { get; } = Brushes.Gold;

    public static ThemeVariantScope BuildScope()
    {
        var palette = new ResourceDictionary();

        // One key, defined once per variant - which is what lets the switch work
        // without anything reassigning a brush.
        palette.ThemeDictionaries[ThemeVariant.Light] =
            new ResourceDictionary { [AccentKey] = LightAccent };
        palette.ThemeDictionaries[ThemeVariant.Dark] =
            new ResourceDictionary { [AccentKey] = DarkAccent };

        var content = new StackPanel { Children = { Accented("Accented") } };

        var scope = new ThemeVariantScope
        {
            RequestedThemeVariant = ThemeVariant.Light,
            Child = content,
        };

        scope.Resources.MergedDictionaries.Add(palette);
        return scope;
    }

    public static ThemeVariantScope BuildNestedDarkScope(ThemeVariantScope outer)
    {
        var nested = new ThemeVariantScope
        {
            RequestedThemeVariant = ThemeVariant.Dark,
            Child = Accented("Inner"),
        };

        // No dictionary of its own: it inherits the outer one and changes only the
        // variant being asked for.
        ((Panel)outer.Child!).Children.Add(nested);
        return nested;
    }

    private static Border Accented(string name)
    {
        var border = new Border { Name = name, Width = 40, Height = 20 };
        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension(AccentKey));
        return border;
    }
}
