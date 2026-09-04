// Exercise 030 - Property Change Observers (beginner).
// Goal:   Watch a dependency property on an object you do not own.
// Drills: DependencyObject.RegisterPropertyChangedCallback and the token it returns,
//         UnregisterPropertyChangedCallback, and why this exists next to the metadata
//         callback from ex002.
// Passes: dotnet test --filter FullyQualifiedName~Ex030_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>
/// Records every value a watched property takes, for as long as the watch is open.
/// </summary>
public sealed class Ex030_PropertyChangeObservers : IDisposable
{
    private readonly List<object?> _values = [];
    private readonly DependencyObject _target;
    private readonly DependencyProperty _property;
    private long _token;
    private bool _disposed;

    /// <summary>
    /// Starts watching <paramref name="property"/> on <paramref name="target"/>. The
    /// current value is not recorded - only changes from here on.
    /// </summary>
    public Ex030_PropertyChangeObservers(DependencyObject target, DependencyProperty property)
    {
        _target = target;
        _property = property;

        // The callback gets the sender and the property, not the old and new values - so
        // read the value off the object. The return value is a token, and it is the only
        // handle that can ever cancel this subscription: there is no "-=" here.
        _token = target.RegisterPropertyChangedCallback(
            property,
            (sender, changed) => _values.Add(sender.GetValue(changed)));
    }

    /// <summary>The values seen since the watch started, in order.</summary>
    public IReadOnlyList<object?> Values => _values;

    /// <summary>
    /// Stops watching. Later changes are not recorded, and calling this twice is harmless.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        // The registration holds a delegate that closes over this observer, so the target
        // keeps it alive until this runs. An observer attached to a long-lived element and
        // never disposed is a leak with no visible symptom.
        _target.UnregisterPropertyChangedCallback(_property, _token);
    }
}
