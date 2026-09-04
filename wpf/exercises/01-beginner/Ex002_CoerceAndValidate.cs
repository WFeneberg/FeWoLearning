// Exercise 002 - Coerce and validate (beginner).
// Goal:   Move the clamping and rejection a legacy setter did by hand into the
//         property system, so styles, bindings and animations go through it too.
// Drills: ValidateValueCallback (reject outright), CoerceValueCallback (clamp into
//         range), PropertyChangedCallback, CoerceValue to re-run coercion, and the
//         order the three callbacks run in.
// Passes: dotnet test --filter FullyQualifiedName~Ex002_

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex002_CoerceAndValidate : DependencyObject
{
    // TODO: register "Volume", type int, owner Ex002_CoerceAndValidate, with
    //   - default value 50,
    //   - a PropertyChangedCallback that appends (oldValue, newValue) to Changes,
    //   - a CoerceValueCallback that clamps the value into [0, Maximum],
    //   - a ValidateValueCallback that returns false for anything below -1000
    //     (WPF turns that into an ArgumentException before coercion ever runs).
    // Expose it as public static readonly DependencyProperty VolumeProperty.

    // TODO: register "Maximum", type int, owner Ex002_CoerceAndValidate, default 100,
    // with a PropertyChangedCallback that calls CoerceValue(VolumeProperty) on the
    // owner - lowering the ceiling has to pull an out-of-range Volume down with it.
    // Expose it as public static readonly DependencyProperty MaximumProperty.

    /// <summary>Current volume. Clamped into [0, <see cref="Maximum"/>].</summary>
    public int Volume
    {
        // TODO: read Volume out of the dependency property.
        get => throw new NotImplementedException("TODO: Ex002 - read Volume from the dependency property");

        // TODO: write Volume into the dependency property.
        set => throw new NotImplementedException("TODO: Ex002 - write Volume into the dependency property");
    }

    /// <summary>Upper bound for <see cref="Volume"/>; 100 by default.</summary>
    public int Maximum
    {
        // TODO: read Maximum out of the dependency property.
        get => throw new NotImplementedException("TODO: Ex002 - read Maximum from the dependency property");

        // TODO: write Maximum into the dependency property.
        set => throw new NotImplementedException("TODO: Ex002 - write Maximum into the dependency property");
    }

    /// <summary>
    /// Every effective change to <see cref="Volume"/>, oldest first. The list itself is
    /// ready to use - the PropertyChangedCallback is what has to fill it.
    /// </summary>
    public List<(int OldValue, int NewValue)> Changes { get; } = [];
}
