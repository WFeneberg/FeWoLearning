// Exercise 001 - Notify By Hand (beginner).
// Goal:   Write INotifyPropertyChanged once, by hand, so ex002 can show you what
//         Caliburn's PropertyChangedBase replaces.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, announcing a computed property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex001_NotifyByHand : INotifyPropertyChanged
{
    private string _firstName = "";
    private string _lastName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (Set(ref _firstName, value)) Raise(nameof(FullName));
        }
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (Set(ref _lastName, value)) Raise(nameof(FullName));
        }
    }

    /// <summary>
    /// Computed, so it has no setter of its own to announce from. A binding to FullName
    /// goes stale unless the two setters above announce it as well.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Returns whether the value actually moved, so callers can chain the dependent
    /// notifications without repeating the comparison.
    /// </summary>
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;

        field = value;
        Raise(propertyName);
        return true;
    }

    private void Raise(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
