// Exercise 030 - Property Change Observers (beginner).
// Goal:   Watch a dependency property on an object you do not own.
// Drills: DependencyObject.RegisterPropertyChangedCallback and the token it returns,
//         UnregisterPropertyChangedCallback, and why this exists next to the metadata
//         callback from ex002.
// Passes: dotnet test --filter FullyQualifiedName~Ex030_
//
// A PropertyMetadata callback is set once, by whoever registers the property - the owner.
// This is the other side: any number of observers, on any instance, at runtime. It is how
// a behaviour or an attached helper reacts to a framework property it did not define, and
// the token is the only thing that can undo the subscription.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>
/// Records every value a watched property takes, for as long as the watch is open.
/// </summary>
public sealed class Ex030_PropertyChangeObservers : IDisposable
{
    private readonly List<object?> _values = [];

    /// <summary>
    /// Starts watching <paramref name="property"/> on <paramref name="target"/>. The
    /// current value is not recorded - only changes from here on.
    /// </summary>
    public Ex030_PropertyChangeObservers(DependencyObject target, DependencyProperty property) =>
        // TODO: register a callback that appends the new value to _values, and keep both
        // the target and the token it returns - unsubscribing needs all three.
        throw new NotImplementedException("TODO: Ex030 - subscribe to the property");

    /// <summary>The values seen since the watch started, in order.</summary>
    public IReadOnlyList<object?> Values => _values;

    /// <summary>
    /// Stops watching. Later changes are not recorded, and calling this twice is harmless.
    /// </summary>
    public void Dispose() =>
        // TODO: unregister with the token. Forgetting this keeps the target alive through
        // the callback for as long as this observer lives.
        throw new NotImplementedException("TODO: Ex030 - unsubscribe with the token");
}
