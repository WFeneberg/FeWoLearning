// Exercise 040 - Hardening the observable view-model base (intermediate). REFERENCE SOLUTION.
// Goal:   Take the SetProperty every earlier exercise built (row 003) and the fan-out
//         pattern built on top of it (row 010), and harden both: let a caller supply its
//         own IEqualityComparer<T> instead of always taking EqualityComparer<T>.Default,
//         and stop a handler that reacts to a property change by setting that SAME
//         property again from causing a second, nested PropertyChanged raise for it.
// Drills: SetProperty with an explicit IEqualityComparer<T>, the dependent-property
//         fan-out from row 010 still working unchanged on top of the hardened base, and a
//         reentrancy guard scoped to the property currently being raised.
// Passes: dotnet test --filter FullyQualifiedName~Ex040_

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public abstract class Ex040_ObservableViewModelBase : INotifyPropertyChanged
{
    private readonly HashSet<string> _raising = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Assigns <paramref name="value"/> to <paramref name="field"/> if it changed under
    /// <paramref name="comparer"/> (or <see cref="EqualityComparer{T}.Default"/> when
    /// <paramref name="comparer"/> is null), raises <see cref="PropertyChanged"/> for the
    /// caller's property name, and returns whether anything changed.
    ///
    /// Reentrancy: a handler reacting to this same property's PropertyChanged is allowed
    /// to call SetProperty again for the SAME property name while that first raise is
    /// still on the call stack. The field must still end up holding whatever that
    /// reentrant call assigned, but this method must not let that reentrant call trigger a
    /// SECOND, nested PropertyChanged for the property already in the middle of raising -
    /// the <see cref="_raising"/> set is provided as the obvious place to track that. A
    /// DIFFERENT property name raising from inside that same handler must be entirely
    /// unaffected - the guard is scoped per property name, not a single global flag.
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, IEqualityComparer<T>? comparer = null, [CallerMemberName] string? propertyName = null)
    {
        var effectiveComparer = comparer ?? EqualityComparer<T>.Default;
        if (effectiveComparer.Equals(field, value))
        {
            return false;
        }

        field = value;

        if (propertyName is not null && _raising.Add(propertyName))
        {
            try
            {
                RaisePropertyChanged(propertyName);
            }
            finally
            {
                _raising.Remove(propertyName);
            }
        }

        return true;
    }

    /// <summary>Raises PropertyChanged directly - for a computed property fanning out from
    /// a field SetProperty already assigned. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
