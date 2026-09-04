// Exercise 048 - Observable Object Base (intermediate).
// Goal:   Write the INPC base class every view model in the app will inherit.
// Drills: a protected Set<T> with [CallerMemberName], an equality guard that reports
//         whether anything moved, and declaring the dependent properties a setter affects.
// Passes: dotnet test --filter FullyQualifiedName~Ex048_
//
// ex004 did this once by hand. Doing it once per view model is how the equality guard gets
// forgotten in half of them, so it belongs in a base class - and the base is where the
// dependent-property problem has to be solved too, or every computed property grows its own
// hand-written notification.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public abstract class Ex048_ObservableObjectBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Assigns <paramref name="value"/> to <paramref name="field"/> if it differs, announces
    /// the calling property and each name in <paramref name="alsoNotify"/>, and returns
    /// whether anything changed.
    /// </summary>
    protected bool Set<T>(
        ref T field,
        T value,
        string[]? alsoNotify = null,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            // Nothing moved, so nothing is announced - not the property and not its
            // dependents. Assigning first and announcing anyway is the usual half-fix, and
            // it re-runs every converter bound to the computed property.
            return false;
        }

        field = value;

        // The property first: a handler that reads a computed value must see the new state.
        Raise(propertyName);

        foreach (var dependent in alsoNotify ?? [])
        {
            Raise(dependent);
        }

        return true;
    }

    /// <summary>Raises <see cref="PropertyChanged"/> for one property.</summary>
    protected void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>
/// A view model on top of the base, to show what using it looks like.
/// </summary>
public sealed class Ex048_Person : Ex048_ObservableObjectBase
{
    private string _first = "";
    private string _last = "";

    /// <summary>Announces itself and <see cref="FullName"/>.</summary>
    public string First
    {
        get => _first;
        // The dependents are declared at the call site, next to the property they belong
        // to - the base cannot know that FullName reads this one.
        set => Set(ref _first, value, [nameof(FullName)]);
    }

    /// <summary>Announces itself and <see cref="FullName"/>.</summary>
    public string Last
    {
        get => _last;
        set => Set(ref _last, value, [nameof(FullName)]);
    }

    /// <summary>Computed, so it depends on both setters announcing it.</summary>
    public string FullName => $"{First} {Last}".Trim();
}
