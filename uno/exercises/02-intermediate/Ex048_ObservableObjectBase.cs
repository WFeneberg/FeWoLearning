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
        [CallerMemberName] string? propertyName = null) =>
        // TODO: compare with EqualityComparer<T>.Default, assign, then raise for the caller
        // and for every dependent name. Return false when nothing moved - and raise nothing
        // in that case, including for the dependents.
        throw new NotImplementedException("TODO: Ex048 - guard, assign and announce");

    /// <summary>Raises <see cref="PropertyChanged"/> for one property.</summary>
    protected void Raise([CallerMemberName] string? propertyName = null) =>
        throw new NotImplementedException("TODO: Ex048 - raise the event");
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
        set => throw new NotImplementedException("TODO: Ex048 - set First through the base");
    }

    /// <summary>Announces itself and <see cref="FullName"/>.</summary>
    public string Last
    {
        get => _last;
        set => throw new NotImplementedException("TODO: Ex048 - set Last through the base");
    }

    /// <summary>Computed, so it depends on both setters announcing it.</summary>
    public string FullName => $"{First} {Last}".Trim();
}
