// Exercise 002 - PropertyChangedBase Basics (beginner).
// Goal:   The same view model as ex001, on Caliburn's base class instead of by hand.
// Drills: PropertyChangedBase, the protected Set helper, NotifyOfPropertyChange for a
//         computed property, and Refresh() as "everything changed".
// Passes: dotnet test --filter FullyQualifiedName~Ex002_
//
// Compare this file with ex001 when you are done. That is the point of the pair.

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
        // TODO: use the inherited Set(ref field, value) helper. It already compares, already
        // announces, and returns whether the value moved - use that to announce FullName too.
        set => throw new NotImplementedException("TODO: Ex002 - set _firstName via Set(...)");
    }

    /// <summary>Announces itself - and <see cref="FullName"/> - but only on a real change.</summary>
    public string LastName
    {
        get => _lastName;
        set => throw new NotImplementedException("TODO: Ex002 - set _lastName via Set(...)");
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Re-announces every property at once, for the case where a view model changed
    /// underneath the bindings and naming each property individually is not worth it.
    /// </summary>
    public void RefreshAll() =>
        throw new NotImplementedException("TODO: Ex002 - announce all properties in one event");
}
