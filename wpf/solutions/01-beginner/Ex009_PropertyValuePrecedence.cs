// Exercise 009 - Property value precedence (beginner). REFERENCE SOLUTION.
// Goal:   See the ladder WPF climbs down every time it resolves a property's effective
//         value - a local value first, then a style setter, then the registered
//         default - and how to ask a DependencyObject which rung it landed on.
// Drills: how a local value, a Style setter and the registered default compete for the
//         same property, and DependencyPropertyHelper.GetValueSource to find out which
//         one actually won.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex009_Badge : FrameworkElement
{
    public static readonly DependencyProperty ToneProperty = DependencyProperty.Register(
        nameof(Tone),
        typeof(string),
        typeof(Ex009_Badge),
        new PropertyMetadata("Neutral"));

    /// <summary>The badge's tone; "Neutral" unless a style or a local value overrides it.</summary>
    public string Tone
    {
        get => (string)GetValue(ToneProperty);
        set => SetValue(ToneProperty, value);
    }

    /// <summary>Which rung of the precedence ladder is currently supplying <paramref name="badge"/>'s Tone.</summary>
    public static BaseValueSource SourceOf(Ex009_Badge badge)
        => DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource;
}
