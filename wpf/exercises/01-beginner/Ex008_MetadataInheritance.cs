// Exercise 008 - Metadata inheritance (beginner).
// Goal:   Make a value set on an ancestor answer GetValue on any descendant with no
//         tree walk at all - the opposite of the manual walk ex007 had to write - and
//         have a change to it automatically invalidate whoever measured against it.
// Drills: FrameworkPropertyMetadata (not the plain PropertyMetadata every earlier
//         exercise used), FrameworkPropertyMetadataOptions.Inherits and
//         FrameworkPropertyMetadataOptions.AffectsMeasure, combined on one property.
// Passes: dotnet test --filter FullyQualifiedName~Ex008_

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex008_MetadataInheritance
{
    // TODO: register an attached property - name "Indent", type double, owner
    // Ex008_MetadataInheritance, default value 0.0 - via DependencyProperty.RegisterAttached,
    // passing a FrameworkPropertyMetadata with FrameworkPropertyMetadataOptions.Inherits |
    // FrameworkPropertyMetadataOptions.AffectsMeasure. Expose it as public static readonly
    // DependencyProperty IndentProperty.
    //
    // Note: Inherits only actually flows to unrelated descendant types when the property
    // is attached (RegisterAttached), the same way FontSize and DataContext are - a plain
    // Register on one owning class, even flagged Inherits, does not propagate the same way.

    /// <summary>Reads the Indent in effect for <paramref name="element"/> - its own value
    /// if it has one, otherwise whatever an ancestor set.</summary>
    public static double GetIndent(DependencyObject element)
        // TODO: return (double)element.GetValue(IndentProperty).
        => throw new NotImplementedException("TODO: Ex008 - read Indent via GetValue");

    /// <summary>Sets Indent on <paramref name="element"/>, which then flows down to any
    /// descendant that does not set its own.</summary>
    public static void SetIndent(DependencyObject element, double value)
        // TODO: element.SetValue(IndentProperty, value).
        => throw new NotImplementedException("TODO: Ex008 - write Indent via SetValue");
}

/// <summary>
/// A minimal element whose desired width grows with whatever Indent is in effect for it.
/// Ready to use - MeasureOverride is not the TODO here, the registration above is; this
/// is only the probe that makes AffectsMeasure and Inherits observable.
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
