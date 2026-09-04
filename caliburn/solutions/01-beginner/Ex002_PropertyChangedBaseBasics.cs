// Exercise 002 - PropertyChangedBase Basics (beginner).
// Goal:   The same view model as ex001, on Caliburn's base class instead of by hand.
// Drills: PropertyChangedBase, the protected Set helper, NotifyOfPropertyChange for a
//         computed property, and Refresh() as "everything changed".
// Passes: dotnet test --filter FullyQualifiedName~Ex002_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex002_PropertyChangedBaseBasics : PropertyChangedBase
{
    private string _firstName = "";
    private string _lastName = "";

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (Set(ref _firstName, value)) NotifyOfPropertyChange(nameof(FullName));
        }
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (Set(ref _lastName, value)) NotifyOfPropertyChange(nameof(FullName));
        }
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Re-announces every property at once, for the case where a view model changed
    /// underneath the bindings and naming each property individually is not worth it.
    /// </summary>
    public void RefreshAll() => Refresh();
}
