using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 038 - OaphInitialValue (intermediate).
/// Goal:   Wire an ObservableAsPropertyHelper with an explicit initial value AND
///         a deferred subscription - the two extra ToProperty parameters beyond
///         the plain Ex037 case.
/// Drills: OAPH initial value, deferred subscription.
///
/// Measured on this machine against ReactiveUI 24.1.0: with deferSubscription
/// left at its default (false), ToProperty subscribes to the source the moment
/// the view model is constructed - before anyone ever reads the property. With
/// deferSubscription: true, nothing is subscribed until the FIRST read of the
/// property, and until then the property reads back exactly the initialValue
/// you passed in. This is the whole point of the exercise: the tests emit from
/// the source BEFORE any read happens, and a wrongly-eager subscription would
/// already have consumed and lost that emission.
/// Passes: dotnet test --filter FullyQualifiedName~Ex038_
public class Ex038_OaphInitialValueViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> _value;
    public int Value => _value.Value;

    /// <summary>
    /// TODO: wire _value as
    ///   source.ToProperty(this, x => x.Value, initialValue, deferSubscription: true);
    /// Forward the given initialValue exactly - do not hard-code a different one.
    /// </summary>
    public Ex038_OaphInitialValueViewModel(IObservable<int> source, int initialValue)
    {
        throw new NotImplementedException(
            "TODO: Ex038 - wire Value via source.ToProperty(this, x => x.Value, initialValue, deferSubscription: true)");
    }
}
