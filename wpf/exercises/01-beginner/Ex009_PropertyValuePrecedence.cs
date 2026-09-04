// Exercise 009 - Property value precedence (beginner).
// Goal:   See the ladder WPF climbs down every time it resolves a property's effective
//         value - a local value first, then a style setter, then the registered
//         default - and how to ask a DependencyObject which rung it landed on.
// Drills: how a local value, a Style setter and the registered default compete for the
//         same property, and DependencyPropertyHelper.GetValueSource to find out which
//         one actually won.
// Passes: dotnet test --filter FullyQualifiedName~Ex009_
//
// Note: there is nothing new to implement here beyond a plain registration - precedence
// is a mechanism WPF already runs for every dependency property. The exercise is in the
// test file: proving it, not building it.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex009_Badge : FrameworkElement
{
    // TODO: register "Tone", type string, owner Ex009_Badge, default value "Neutral".
    // Expose it as public static readonly DependencyProperty ToneProperty.

    /// <summary>The badge's tone; "Neutral" unless a style or a local value overrides it.</summary>
    public string Tone
    {
        // TODO: read Tone out of the dependency property.
        get => throw new NotImplementedException("TODO: Ex009 - read Tone from the dependency property");

        // TODO: write Tone into the dependency property.
        set => throw new NotImplementedException("TODO: Ex009 - write Tone into the dependency property");
    }
}
