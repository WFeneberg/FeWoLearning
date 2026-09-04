// Exercise 007 - Value Converter (beginner).
// Goal:   Turn a source value into something an element can display, and back again.
// Drills: IValueConverter, the WinUI signature (a `string language`, not a CultureInfo),
//         DependencyProperty.UnsetValue for input the converter cannot handle.
// Passes: dotnet test --filter FullyQualifiedName~Ex007_

using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>Formats a number with the thousands and decimal separators of a language.</summary>
public class Ex007_ValueConverter : IValueConverter
{
    /// <summary>
    /// A double becomes its "N2" text in the given language: 1234.5 is "1,234.50" in
    /// "en-US" and "1.234,50" in "de-DE". An empty language means invariant culture.
    /// Anything that is not a double returns <see cref="DependencyProperty.UnsetValue"/>,
    /// which tells the binding engine to fall back instead of showing garbage.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is double number
            ? number.ToString("N2", CultureFor(language))
            : DependencyProperty.UnsetValue;

    /// <summary>
    /// The reverse: parse the text in the same language back into a double. Text that does
    /// not parse returns <see cref="DependencyProperty.UnsetValue"/>.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is string text
        && double.TryParse(text, NumberStyles.Number, CultureFor(language), out var number)
            ? number
            : DependencyProperty.UnsetValue;

    /// <summary>
    /// The language is a BCP-47 tag, and it is empty far more often than one expects -
    /// nothing on the element has to set Language for a binding to run.
    /// </summary>
    private static CultureInfo CultureFor(string language) =>
        string.IsNullOrEmpty(language)
            ? CultureInfo.InvariantCulture
            : CultureInfo.GetCultureInfo(language);
}
