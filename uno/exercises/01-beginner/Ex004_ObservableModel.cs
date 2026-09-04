// Exercise 004 - Observable Model (beginner).
// Goal:   A view model the binding engine can actually follow.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, and notifying a *computed* property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_
//
// This is not a DependencyObject: view models stay POCOs. Dependency properties are for
// the elements on the other end of the binding.

using System.ComponentModel;

namespace FeWoLearning.Uno.Exercises.Beginner;

public class Ex004_ObservableModel : INotifyPropertyChanged
{
    private string _name = "";
    private int _age;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raises PropertyChanged - but only when the value really moved.</summary>
    public string Name
    {
        get => _name;
        set => throw new NotImplementedException("TODO: Ex004 - set _name and notify");
    }

    /// <summary>Raises PropertyChanged - but only when the value really moved.</summary>
    public int Age
    {
        get => _age;
        set => throw new NotImplementedException("TODO: Ex004 - set _age and notify");
    }

    /// <summary>
    /// Computed, so it has no setter to notify from. A binding to Summary only refreshes
    /// if Name and Age also announce Summary when they change.
    /// </summary>
    public string Summary => $"{Name} ({Age})";

    // TODO: add one helper both setters use - assign through a `ref` field, compare with
    // EqualityComparer<T>.Default, and let the caller name come from [CallerMemberName]
    // so no setter passes a string literal.
}
