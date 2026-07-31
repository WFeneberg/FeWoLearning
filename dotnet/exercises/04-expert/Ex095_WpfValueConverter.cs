using System.Globalization;

namespace FeWoLearning.Exercises.Expert;

// Exercise 095 — WPF-style value converter & binding (expert).
// Goal:   Implement a two-way IValueConverter that maps bool <-> a
//         Visibility-like enum, the way a WPF "BooleanToVisibilityConverter"
//         would for a binding such as {Binding IsOn, Converter={StaticResource
//         WpfValueConverter}}. Support a "parameter" that inverts the mapping
//         (mirrors the WPF idiom of passing ConverterParameter="Invert"), and
//         fail gracefully — rather than throwing — on input that cannot be
//         converted, by returning the WPF-style Binding.DoNothing sentinel.
// Drills: IValueConverter Convert/ConvertBack contract, ConverterParameter
//         handling, defensive type-checking at a binding boundary.
//
// Note: this project targets plain net10.0 (no WPF/UseWPF), so the WPF types
// System.Windows.Data.IValueConverter, System.Windows.Visibility and
// System.Windows.Data.Binding.DoNothing are mirrored locally below with the
// same shape/semantics they have in WPF, keeping the exercise dependency-free
// while still exercising the exact conversion logic a real WPF app would need.

// Mirrors System.Windows.Visibility.
public enum Visibility
{
    Visible,
    Hidden,
    Collapsed,
}

// Mirrors System.Windows.Data.IValueConverter.
public interface IValueConverter
{
    object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);
}

public sealed class WpfValueConverter : IValueConverter
{
    // Mirrors System.Windows.Data.Binding.DoNothing: returned instead of
    // throwing when the converter cannot make sense of its input, telling
    // the binding engine to leave the target property untouched.
    public static readonly object DoNothing = new();

    // true  -> Visibility.Visible  (or Visibility.Collapsed when parameter is "Invert")
    // false -> Visibility.Collapsed (or Visibility.Visible when parameter is "Invert")
    // Anything other than a bool value returns DoNothing.
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    // Visibility.Visible            -> true  (or false when parameter is "Invert")
    // Visibility.Hidden/Collapsed   -> false (or true when parameter is "Invert")
    // Anything other than a Visibility value returns DoNothing.
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
