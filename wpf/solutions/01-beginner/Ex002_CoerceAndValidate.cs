// Exercise 002 - Coerce and validate (beginner). REFERENCE SOLUTION.
// Goal:   Move the clamping and rejection a legacy setter did by hand into the
//         property system, so styles, bindings and animations go through it too.
// Drills: ValidateValueCallback (reject outright), CoerceValueCallback (clamp into
//         range), PropertyChangedCallback, CoerceValue to re-run coercion, and the
//         order the three callbacks run in.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex002_CoerceAndValidate : DependencyObject
{
    public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
        nameof(Volume),
        typeof(int),
        typeof(Ex002_CoerceAndValidate),
        new PropertyMetadata(50, OnVolumeChanged, CoerceVolume),
        ValidateVolume);

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(Ex002_CoerceAndValidate),
        new PropertyMetadata(100, OnMaximumChanged));

    /// <summary>Current volume. Clamped into [0, <see cref="Maximum"/>].</summary>
    public int Volume
    {
        get => (int)GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, value);
    }

    /// <summary>Upper bound for <see cref="Volume"/>; 100 by default.</summary>
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>Every effective change to <see cref="Volume"/>, oldest first.</summary>
    public List<(int OldValue, int NewValue)> Changes { get; } = [];

    // Runs first, and on the raw value: a hard reject, not a clamp. Returning false
    // makes SetValue throw ArgumentException and leaves the store untouched.
    private static bool ValidateVolume(object value) => (int)value >= -1000;

    // Runs second. Never writes to the store - it only reports the value the store
    // should expose, so the local value survives underneath and comes back when the
    // constraint relaxes.
    private static object CoerceVolume(DependencyObject d, object baseValue)
    {
        var mixer = (Ex002_CoerceAndValidate)d;
        var requested = (int)baseValue;

        return Math.Clamp(requested, 0, mixer.Maximum);
    }

    // Runs last, and on the coerced value.
    private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var mixer = (Ex002_CoerceAndValidate)d;

        mixer.Changes.Add(((int)e.OldValue, (int)e.NewValue));
    }

    // Volume's coercion depends on Maximum, and the property system does not know that.
    // Saying so explicitly is what keeps the two consistent.
    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(VolumeProperty);
    }
}
