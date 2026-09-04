// Exercise 003 - NotifyOfPropertyChange (beginner).
// Goal:   Announce a property whose value does not live in a field of this class.
// Drills: NotifyOfPropertyChange, why Set(ref ...) cannot help here, doing the equality
//         check yourself, announcing a dependent computed property.
// Passes: dotnet test --filter FullyQualifiedName~Ex003_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex003_NotifyOfPropertyChange(IDictionary<string, string> store) : PropertyChangedBase
{
    private const string Key = "Theme";

    /// <summary>Reads through to the store; "light" when the store has nothing yet.</summary>
    public string Theme
    {
        get => store.TryGetValue(Key, out var value) ? value : "light";
        set
        {
            // Compare against what the getter would return, not against the raw store
            // entry: writing "light" into an empty store changes nothing observable.
            if (Theme == value) return;

            store[Key] = value;
            NotifyOfPropertyChange(nameof(Theme));
            NotifyOfPropertyChange(nameof(IsDark));
        }
    }

    /// <summary>Computed from <see cref="Theme"/>, so only the Theme setter can announce it.</summary>
    public bool IsDark => Theme == "dark";
}
