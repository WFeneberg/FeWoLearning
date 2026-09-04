// Exercise 007 - Attached property (beginner). REFERENCE SOLUTION.
// Goal:   Attach a value to *any* element rather than only to instances of one class -
//         the shape Grid.Row and Panel.ZIndex take - and read it back from a
//         descendant instead of only from the element it was set on.
// Drills: DependencyProperty.RegisterAttached, the static GetX/SetX accessor pair an
//         attached property is read and written through instead of a CLR property,
//         and walking up the visual tree to read a value set on a parent.

using System.Windows;
using System.Windows.Media;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex007_AttachedProperty
{
    public static readonly DependencyProperty SectionProperty = DependencyProperty.RegisterAttached(
        "Section",
        typeof(string),
        typeof(Ex007_AttachedProperty),
        new PropertyMetadata(null));

    /// <summary>Reads the Section attached to <paramref name="element"/> itself. Null if
    /// nothing was ever attached there.</summary>
    public static string? GetSection(DependencyObject element) => (string?)element.GetValue(SectionProperty);

    /// <summary>Attaches <paramref name="value"/> to <paramref name="element"/> -
    /// typically a container several children sit under.</summary>
    public static void SetSection(DependencyObject element, string? value) => element.SetValue(SectionProperty, value);

    /// <summary>
    /// Walks up the visual tree starting at <paramref name="element"/> itself and
    /// returns the first non-null Section found on it or an ancestor. Returns null if
    /// nobody in the chain up to the root has one.
    /// </summary>
    public static string? GetEffectiveSection(DependencyObject element)
    {
        DependencyObject? current = element;

        while (current is not null)
        {
            var section = GetSection(current);
            if (section is not null)
            {
                return section;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
