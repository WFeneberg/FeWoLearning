using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 036 - WhenAnyValueMultiArity (intermediate).
/// Goal:   Combine several source properties into one derived property using a
///         single multi-arity WhenAnyValue call, not one subscription per source.
/// Drills: WhenAnyValue over several source properties.
/// Passes: dotnet test --filter FullyQualifiedName~Ex036_
public class Ex036_WhenAnyValueMultiArityViewModel : ReactiveObject
{
    private string _firstName = "Ada";
    public string FirstName { get => _firstName; set => this.RaiseAndSetIfChanged(ref _firstName, value); }

    private string _lastName = "Lovelace";
    public string LastName { get => _lastName; set => this.RaiseAndSetIfChanged(ref _lastName, value); }

    private int _age = 28;
    public int Age { get => _age; set => this.RaiseAndSetIfChanged(ref _age, value); }

    private string _summary = string.Empty;
    public string Summary { get => _summary; private set => this.RaiseAndSetIfChanged(ref _summary, value); }

    /// <summary>
    /// TODO: in this constructor, subscribe to the THREE-source overload of
    /// WhenAnyValue - this.WhenAnyValue(x => x.FirstName, x => x.LastName,
    /// x => x.Age, (f, l, a) => $"{f} {l} ({a})") - and assign each emission to
    /// Summary. Do not wire FirstName, LastName and Age with three separate
    /// single-property WhenAnyValue subscriptions instead: a real mistake here is
    /// reacting to only one or two of the three, which looks right until you
    /// change one of the OTHERS on its own and Summary silently goes stale. The
    /// tests change each of the three properties independently, one at a time,
    /// and check Summary after every single change.
    /// </summary>
    public Ex036_WhenAnyValueMultiArityViewModel()
    {
        throw new NotImplementedException(
            "TODO: Ex036 - wire Summary from a 3-source WhenAnyValue(FirstName, LastName, Age)");
    }
}
