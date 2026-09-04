// Exercise 007 - Value Converter (beginner).
// Goal:   Turn a source value into something an element can display, and back again.
// Drills: IValueConverter, the WinUI signature (a `string language`, not a CultureInfo),
//         DependencyProperty.UnsetValue for input the converter cannot handle.
// Passes: dotnet test --filter FullyQualifiedName~Ex007_
//
// Watch the signature. WPF's IValueConverter takes a CultureInfo; WinUI and Uno hand you
// a BCP-47 language tag as a string, and an empty string when nobody set one.

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
        throw new NotImplementedException("TODO: Ex007 - format the double for the given language");

    /// <summary>
    /// The reverse: parse the text in the same language back into a double. Text that does
    /// not parse returns <see cref="DependencyProperty.UnsetValue"/>.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("TODO: Ex007 - parse the text back into a double");
}
