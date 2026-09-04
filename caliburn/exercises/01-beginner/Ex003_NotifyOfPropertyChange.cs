// Exercise 003 - NotifyOfPropertyChange (beginner).
// Goal:   Announce a property whose value does not live in a field of this class.
// Drills: NotifyOfPropertyChange, why Set(ref ...) cannot help here, doing the equality
//         check yourself, announcing a dependent computed property.
// Passes: dotnet test --filter FullyQualifiedName~Ex003_
//
// Set(ref field, value) needs a backing FIELD to take by reference. This view model keeps
// its value in an injected store instead - which is the normal case as soon as a setting
// is shared, persisted, or owned by a service.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex003_NotifyOfPropertyChange(IDictionary<string, string> store) : PropertyChangedBase
{
    private const string Key = "Theme";

    /// <summary>Reads through to the store; "light" when the store has nothing yet.</summary>
    public string Theme
    {
        get => store.TryGetValue(Key, out var value) ? value : "light";

        // TODO: write the value into the store under Key, then announce Theme and IsDark.
        // Announce nothing at all when the incoming value equals the current one - there is
        // no Set(...) here to do that comparison for you.
        set => throw new NotImplementedException("TODO: Ex003 - store the theme and announce it");
    }

    /// <summary>Computed from <see cref="Theme"/>, so only the Theme setter can announce it.</summary>
    public bool IsDark => Theme == "dark";
}
