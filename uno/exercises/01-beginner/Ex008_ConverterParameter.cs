// Exercise 008 - Converter Parameter (beginner).
// Goal:   Reuse one converter with different settings, passed per binding.
// Drills: ConverterParameter, and why a parameter that arrives as text must be parsed
//         with the invariant culture rather than the current one.
// Passes: dotnet test --filter FullyQualifiedName~Ex008_
//
// The parameter comes out of the markup, so "2.5" is a XAML literal, not a localised
// number. Parsing it with CurrentCulture works on your machine and breaks on a German
// one, where "2.5" means two and a half thousand.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>Shows an element only while a number is at or above a threshold.</summary>
public class Ex008_ConverterParameter : IValueConverter
{
    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> when the double <paramref name="value"/>
    /// is greater than or equal to the threshold in <paramref name="parameter"/>, and
    /// <see cref="Visibility.Collapsed"/> otherwise. The threshold may arrive as a string
    /// from markup or already as a double from code; a missing or unparsable threshold
    /// means "no threshold", so everything is visible.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("TODO: Ex008 - compare value against the threshold");

    /// <summary>A visibility says nothing about the number it came from.</summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("TODO: Ex008 - this direction has no answer");
}
