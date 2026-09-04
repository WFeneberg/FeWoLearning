// Exercise 004 - Observable Model (beginner).
// Goal:   A view model the binding engine can actually follow.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, and notifying a *computed* property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_

using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        set
        {
            if (Set(ref _name, value))
            {
                Raise(nameof(Summary));
            }
        }
    }

    /// <summary>Raises PropertyChanged - but only when the value really moved.</summary>
    public int Age
    {
        get => _age;
        set
        {
            if (Set(ref _age, value))
            {
                Raise(nameof(Summary));
            }
        }
    }

    /// <summary>
    /// Computed, so it has no setter to notify from. A binding to Summary only refreshes
    /// if Name and Age also announce Summary when they change.
    /// </summary>
    public string Summary => $"{Name} ({Age})";

    /// <summary>
    /// Returns whether the value actually moved, so callers can chain the dependent
    /// notifications without repeating the comparison.
    /// </summary>
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // Equality first: a redundant notification re-evaluates every binding on the
        // property and re-runs its converters, for nothing.
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        Raise(propertyName);
        return true;
    }

    private void Raise(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
