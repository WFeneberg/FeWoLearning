// Exercise 007 - Attached property (beginner).
// Goal:   Attach a value to *any* element rather than only to instances of one class -
//         the shape Grid.Row and Panel.ZIndex take - and read it back from a
//         descendant instead of only from the element it was set on.
// Drills: DependencyProperty.RegisterAttached, the static GetX/SetX accessor pair an
//         attached property is read and written through instead of a CLR property,
//         and walking up the visual tree to read a value set on a parent.
// Passes: dotnet test --filter FullyQualifiedName~Ex007_

using System.Windows;
using System.Windows.Media;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex007_AttachedProperty
{
    // TODO: register an attached property - name "Section", type string, owner
    // Ex007_AttachedProperty, default value null - via DependencyProperty.RegisterAttached.
    // Expose it as public static readonly DependencyProperty SectionProperty. Unlike
    // Register, there is no owning instance type: any DependencyObject can carry a value
    // for it, which is the whole point of "attached".

    /// <summary>Reads the Section attached to <paramref name="element"/> itself. Null if
    /// nothing was ever attached there.</summary>
    public static string? GetSection(DependencyObject element)
        // TODO: return element.GetValue(SectionProperty), cast to string?.
        => throw new NotImplementedException("TODO: Ex007 - read Section via GetValue");

    /// <summary>Attaches <paramref name="value"/> to <paramref name="element"/> -
    /// typically a container several children sit under.</summary>
    public static void SetSection(DependencyObject element, string? value)
        // TODO: element.SetValue(SectionProperty, value).
        => throw new NotImplementedException("TODO: Ex007 - write Section via SetValue");

    /// <summary>
    /// Walks up the visual tree starting at <paramref name="element"/> itself and
    /// returns the first non-null Section found on it or an ancestor - the way a child
    /// discovers what section its container belongs to without every child having to
    /// carry the value itself. Returns null if nobody in the chain up to the root has one.
    /// </summary>
    public static string? GetEffectiveSection(DependencyObject element)
    {
        // TODO: starting at element, check GetSection(current); if it is non-null return
        // it, otherwise move to VisualTreeHelper.GetParent(current) and repeat. Stop and
        // return null once GetParent returns null.
        throw new NotImplementedException("TODO: Ex007 - walk VisualTreeHelper.GetParent looking for a set Section");
    }
}
