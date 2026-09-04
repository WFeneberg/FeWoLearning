// Exercise 008 - Metadata inheritance (beginner). REFERENCE SOLUTION.
// Goal:   Make a value set on an ancestor answer GetValue on any descendant with no
//         tree walk at all - the opposite of the manual walk ex007 had to write - and
//         have a change to it automatically invalidate whoever measured against it.
// Drills: FrameworkPropertyMetadata (not the plain PropertyMetadata every earlier
//         exercise used), FrameworkPropertyMetadataOptions.Inherits and
//         FrameworkPropertyMetadataOptions.AffectsMeasure, combined on one property.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex008_MetadataInheritance
{
    public static readonly DependencyProperty IndentProperty = DependencyProperty.RegisterAttached(
        "Indent",
        typeof(double),
        typeof(Ex008_MetadataInheritance),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Reads the Indent in effect for <paramref name="element"/> - its own value
    /// if it has one, otherwise whatever an ancestor set.</summary>
    public static double GetIndent(DependencyObject element) => (double)element.GetValue(IndentProperty);

    /// <summary>Sets Indent on <paramref name="element"/>, which then flows down to any
    /// descendant that does not set its own.</summary>
    public static void SetIndent(DependencyObject element, double value) => element.SetValue(IndentProperty, value);
}

/// <summary>
/// A minimal element whose desired width grows with whatever Indent is in effect for it.
/// </summary>
public class Ex008_IndentBox : FrameworkElement
{
    /// <summary>How many times MeasureOverride has run.</summary>
    public int MeasurePassCount { get; private set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        MeasurePassCount++;

        var indent = Ex008_MetadataInheritance.GetIndent(this);
        return new Size(Math.Min(10 + indent, availableSize.Width), Math.Min(10, availableSize.Height));
    }
}
