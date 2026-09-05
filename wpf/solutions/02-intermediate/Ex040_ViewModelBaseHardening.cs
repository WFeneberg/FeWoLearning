// Exercise 040 - Hardening the observable view-model base (intermediate).
// REFERENCE SOLUTION.
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
    /// Guarded against reentrancy: if a handler reacting to this same property's
    /// PropertyChanged calls SetProperty again for the SAME property name while that first
    /// raise is still in progress, this method must still assign the new field value, but
    /// must NOT raise a second, nested PropertyChanged for that property - the raise
    /// already in progress is the only notification that property gets for this call
    /// stack. A different property name raising from inside that handler is NOT affected
    /// by the guard.
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
