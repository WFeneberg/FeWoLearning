// Exercise 006 - Read-only dependency property (beginner). REFERENCE SOLUTION.
// Goal:   Expose a status flag that consumers can bind or trigger on but never set
//         themselves - only the owning class may change it, from the inside.
// Drills: DependencyProperty.RegisterReadOnly, DependencyPropertyKey, exposing the
//         associated DependencyProperty for read-only consumption, and writing
//         through the key - never through the property - from owner code.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex006_ConnectionMonitor : DependencyObject
{
    private static readonly DependencyPropertyKey IsConnectedPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsConnected),
        typeof(bool),
        typeof(Ex006_ConnectionMonitor),
        new PropertyMetadata(false));

    public static readonly DependencyProperty IsConnectedProperty = IsConnectedPropertyKey.DependencyProperty;

    /// <summary>Whether the monitor currently reports a live connection. Read-only:
    /// only <see cref="Connect"/> and <see cref="Disconnect"/> may change it.</summary>
    public bool IsConnected => (bool)GetValue(IsConnectedProperty);

    /// <summary>Marks the monitor connected. The only legal way IsConnected becomes true.</summary>
    public void Connect() => SetValue(IsConnectedPropertyKey, true);

    /// <summary>Marks the monitor disconnected.</summary>
    public void Disconnect() => SetValue(IsConnectedPropertyKey, false);
}
