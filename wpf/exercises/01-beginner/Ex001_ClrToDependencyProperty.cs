// Exercise 001 - CLR property to dependency property (beginner).
// Goal:   Replace a plain CLR property with a real dependency property - the first
//         thing a migration does when a value has to be styled, bound or animated.
// Drills: DependencyProperty.Register, PropertyMetadata default values,
//         GetValue/SetValue, and ClearValue falling back to the registered default.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_
//
// Note: unlike Uno/WinUI, WPF needs no `partial` here - there is no generated second
// half of the class. DependencyObject is enough.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex001_ClrToDependencyProperty : DependencyObject
{
    // TODO: register a dependency property - name "Threshold", type int, owner
    // Ex001_ClrToDependencyProperty, default value 5. Expose the registration as a
    // public static readonly field called ThresholdProperty. The field IS the
    // property's identity, so register exactly once.

    /// <summary>Alarm threshold; 5 unless somebody sets it.</summary>
    public int Threshold
    {
        // TODO: read the value out of the dependency property. Do not add a backing field.
        get => throw new NotImplementedException("TODO: Ex001 - read Threshold from the dependency property");

        // TODO: write the value into the dependency property.
        set => throw new NotImplementedException("TODO: Ex001 - write Threshold into the dependency property");
    }
}
