// Exercise 023 - Implicit style keyed by type (beginner).
// Goal:   Key a Style by its own TargetType in a dictionary so it applies to every matching
//         element beneath that dictionary automatically - no explicit Style assignment
//         anywhere. This only works because the style lives in an element's own Resources:
//         this harness never constructs an Application (see README - "What the harness
//         cannot do"), so the usual middle stop in implicit-style lookup - Application.
//         Current.Resources - simply is not there. An implicit style with nowhere but an
//         element's own Resources to live in has nowhere to go here.
// Drills: a Style whose TargetType is used as its own dictionary key (the code equivalent
//         of an untagged <Style TargetType="..."> in XAML) - added to an element's
//         Resources, not a Style assigned to any one element directly.
// Passes: dotnet test --filter FullyQualifiedName~Ex023_

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Ready to use - not the subject of this row (the property system itself is rows
/// 001-008's drill). "Plain" unless an implicit style overrides it.
/// </summary>
public class Ex023_Chip : FrameworkElement
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(Ex023_Chip), new FrameworkPropertyMetadata("Plain"));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
}

public static class Ex023_ImplicitStyleByType
{
    /// <summary>
    /// Adds an implicit style to <paramref name="resources"/>: TargetType typeof(Ex023_Chip),
    /// a single Setter for Label = <paramref name="label"/>, keyed by typeof(Ex023_Chip)
    /// itself - the implicit-style key.
    /// </summary>
    public static void AddImplicitChipStyle(ResourceDictionary resources, string label)
        // TODO: var style = new Style(typeof(Ex023_Chip));
        //       style.Setters.Add(new Setter(Ex023_Chip.LabelProperty, label));
        //       resources[typeof(Ex023_Chip)] = style;
        => throw new NotImplementedException("TODO: Ex023 - build a Style with TargetType typeof(Ex023_Chip) and a Setter for Ex023_Chip.LabelProperty = label, then add it to resources keyed by typeof(Ex023_Chip) itself (the implicit-style key)");
}
