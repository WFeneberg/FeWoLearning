using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 038 - OaphInitialValue (intermediate).
/// Goal:   Wire an ObservableAsPropertyHelper with an explicit initial value AND
///         a deferred subscription - the two extra ToProperty parameters beyond
///         the plain Ex037 case.
/// Drills: OAPH initial value, deferred subscription.
/// Passes: dotnet test --filter FullyQualifiedName~Ex038_
public class Ex038_OaphInitialValueViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> _value;
    public int Value => _value.Value;

    public Ex038_OaphInitialValueViewModel(IObservable<int> source, int initialValue)
    {
        _value = source.ToProperty(this, x => x.Value, initialValue, deferSubscription: true);
    }
}
