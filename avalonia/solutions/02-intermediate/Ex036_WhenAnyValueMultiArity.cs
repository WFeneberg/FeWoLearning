using ReactiveUI;
using ReactiveUI.Primitives;

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

    public Ex036_WhenAnyValueMultiArityViewModel()
    {
        this.WhenAnyValue(x => x.FirstName, x => x.LastName, x => x.Age, Format)
            .Subscribe(s => Summary = s);
    }

    private static string Format(string firstName, string lastName, int age) =>
        $"{firstName} {lastName} ({age})";
}
