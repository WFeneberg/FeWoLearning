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

    public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register(
            nameof(Level),
            typeof(int),
            typeof(Ex002_PropertyChangedCallback),
            new PropertyMetadata(0, OnLevelChanged));

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

    // The metadata belongs to the property, not to an instance, so the callback is static
    // and the instance arrives as the first argument. Casting it is safe: the property
    // store only ever hands back an owner of the type the property was registered for.
    private static void OnLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex002_PropertyChangedCallback)sender)._transitions.Add($"{args.OldValue}->{args.NewValue}");
}
