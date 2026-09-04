// Exercise 001 - Notify By Hand (beginner).
// Goal:   Write INotifyPropertyChanged once, by hand, so ex002 can show you what
//         Caliburn's PropertyChangedBase replaces.
// Drills: INotifyPropertyChanged, [CallerMemberName], suppressing the event when the
//         value did not change, announcing a computed property whose inputs moved.
// Passes: dotnet test --filter FullyQualifiedName~Ex001_
//
// Deliberately NOT derived from PropertyChangedBase. This is the only exercise in the
// track that writes the plumbing by hand.

using System.ComponentModel;

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
        set => throw new NotImplementedException("TODO: Ex001 - store _firstName and announce it");
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set => throw new NotImplementedException("TODO: Ex001 - store _lastName and announce it");
    }

    /// <summary>
    /// Computed, so it has no setter of its own to announce from. A binding to FullName
    /// goes stale unless the two setters above announce it as well.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // TODO: add one helper both setters use. Take the field by `ref`, compare with
    // EqualityComparer<T>.Default, return whether the value actually moved, and let the
    // property name arrive through [CallerMemberName] so no setter passes a string literal.
}
