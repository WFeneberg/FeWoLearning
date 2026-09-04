// Exercise 002 - Property Changed Callback (beginner).
// Goal:   React to a dependency property changing, from inside the property metadata.
// Drills: PropertyChangedCallback, DependencyPropertyChangedEventArgs.OldValue/NewValue,
//         the static-callback-to-instance hop, and the fact that the framework does not
//         call you when the value did not actually change.
// Passes: dotnet test --filter FullyQualifiedName~Ex002_

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex002_PropertyChangedCallback : DependencyObject
{
    private readonly List<string> _transitions = [];

    // TODO: give this metadata a PropertyChangedCallback. The callback is static - the
    // instance arrives as the DependencyObject argument, cast it back.
    public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register(
            nameof(Level),
            typeof(int),
            typeof(Ex002_PropertyChangedCallback),
            new PropertyMetadata(0));

    public int Level
    {
        get => (int)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    /// <summary>
    /// One "old-&gt;new" entry per observed change, in order. Stays empty while nothing
    /// changes, and gets no entry for a write that does not move the value.
    /// </summary>
    public IReadOnlyList<string> Transitions => _transitions;

    // TODO: implement the callback and record "old->new" into _transitions, e.g. "0->3".
}
