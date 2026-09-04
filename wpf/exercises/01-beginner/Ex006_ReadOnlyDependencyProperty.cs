// Exercise 006 - Read-only dependency property (beginner).
// Goal:   Expose a status flag that consumers can bind or trigger on but never set
//         themselves - only the owning class may change it, from the inside.
// Drills: DependencyProperty.RegisterReadOnly, DependencyPropertyKey, exposing the
//         associated DependencyProperty for read-only consumption, and writing
//         through the key - never through the property - from owner code.
// Passes: dotnet test --filter FullyQualifiedName~Ex006_

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex006_ConnectionMonitor : DependencyObject
{
    // TODO: register a read-only dependency property - name "IsConnected", type bool,
    // owner Ex006_ConnectionMonitor, default value false - via
    // DependencyProperty.RegisterReadOnly. Keep the returned DependencyPropertyKey in a
    // private static readonly field called IsConnectedPropertyKey: that key is the only
    // handle that can ever write the value.
    //
    // Expose the associated DependencyProperty (key.DependencyProperty) as a public
    // static readonly field called IsConnectedProperty, the way every other exercise
    // exposes its registration - callers may bind or trigger on it, but SetValue on
    // this field must be rejected; only IsConnectedPropertyKey can write.

    /// <summary>Whether the monitor currently reports a live connection. Read-only:
    /// only <see cref="Connect"/> and <see cref="Disconnect"/> may change it.</summary>
    public bool IsConnected
        // TODO: read IsConnected out of the dependency property.
        => throw new NotImplementedException("TODO: Ex006 - read IsConnected from the dependency property");

    /// <summary>Marks the monitor connected. The only legal way IsConnected becomes true.</summary>
    public void Connect()
        // TODO: write true through IsConnectedPropertyKey - never through IsConnectedProperty.
        => throw new NotImplementedException("TODO: Ex006 - set IsConnected to true through the key");

    /// <summary>Marks the monitor disconnected.</summary>
    public void Disconnect()
        // TODO: write false through IsConnectedPropertyKey.
        => throw new NotImplementedException("TODO: Ex006 - set IsConnected to false through the key");
}
