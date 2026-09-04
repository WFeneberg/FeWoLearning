// Exercise 001 - CLR property to dependency property (beginner). REFERENCE SOLUTION.
// Goal:   Replace a plain CLR property with a real dependency property - the first
//         thing a migration does when a value has to be styled, bound or animated.
// Drills: DependencyProperty.Register, PropertyMetadata default values,
//         GetValue/SetValue, and ClearValue falling back to the registered default.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex001_ClrToDependencyProperty : DependencyObject
{
    /// <summary>The identity of the Threshold property. Registered exactly once.</summary>
    public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register(
        nameof(Threshold),
        typeof(int),
        typeof(Ex001_ClrToDependencyProperty),
        new PropertyMetadata(5));

    /// <summary>Alarm threshold; 5 unless somebody sets it.</summary>
    public int Threshold
    {
        get => (int)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }
}
