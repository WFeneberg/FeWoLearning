// Exercise 043 - INotifyDataErrorInfo (intermediate). REFERENCE SOLUTION.
// Goal:   Surface per-property validation errors to a Binding through the modern async-ready
//         interface - ErrorsChanged, GetErrors, HasErrors - and measure its one sharp edge
//         directly rather than from memory: unlike the legacy IDataErrorInfo row right after
//         this one, a Binding validates against INotifyDataErrorInfo by DEFAULT. Measured on
//         this harness: Binding.ValidatesOnNotifyDataErrors defaults to true, and a TextBox
//         bound to a source implementing this interface, with NO flag set anywhere, already
//         shows Validation.GetHasError = true the moment the source reports an error. Row 044
//         needs its own flag named in its own Concepts cell for exactly the opposite reason.
// Drills: INotifyDataErrorInfo (ErrorsChanged, GetErrors, HasErrors), and per-property error
//         storage where HasErrors is derived from the SAME store GetErrors reads - never a
//         separately maintained flag that can drift out of sync across two different
//         properties gaining and losing errors independently.

using System.Collections;
using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ships only the validation plumbing - no concrete validated view model here (one belongs
/// to whoever actually has properties to validate, and shipping one "ready to use" would let
/// this row's whole subject go untested through it instead of through the interface itself).
/// </summary>
public abstract class Ex043_ValidatingViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errorsByProperty = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// True while any property currently has at least one recorded error.
    /// </summary>
    public bool HasErrors => _errorsByProperty.Count > 0;

    /// <summary>
    /// Returns the errors currently recorded for <paramref name="propertyName"/> - empty for
    /// none, and empty for a null/empty propertyName (this row does not model entity-level
    /// errors).
    /// </summary>
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return Array.Empty<string>();
        }

        return _errorsByProperty.TryGetValue(propertyName, out var errors) ? errors : Array.Empty<string>();
    }

    /// <summary>
    /// Replaces the recorded errors for <paramref name="propertyName"/> with
    /// <paramref name="errors"/> (store your own copy - do not hold onto the caller's list
    /// instance). Raise ErrorsChanged for propertyName, via RaiseErrorsChanged below, ONLY if
    /// the recorded set for that property actually changed - passing the same errors again
    /// must raise nothing, the same "no event without a real change" rule every
    /// INotifyPropertyChanged row in this track already follows. If HasErrors' own value
    /// flips as a RESULT of this call, also raise PropertyChanged(nameof(HasErrors)) via
    /// RaisePropertyChanged.
    /// </summary>
    protected void SetErrors(string propertyName, IReadOnlyList<string> errors)
    {
        var hadErrorsBefore = HasErrors;
        var existing = _errorsByProperty.TryGetValue(propertyName, out var current) ? current : null;

        var changed = errors.Count == 0
            ? existing is { Count: > 0 }
            : existing is null || !existing.SequenceEqual(errors);

        if (!changed)
        {
            return;
        }

        if (errors.Count == 0)
        {
            _errorsByProperty.Remove(propertyName);
        }
        else
        {
            _errorsByProperty[propertyName] = errors.ToList();
        }

        RaiseErrorsChanged(propertyName);

        if (HasErrors != hadErrorsBefore)
        {
            RaisePropertyChanged(nameof(HasErrors));
        }
    }

    /// <summary>Raises PropertyChanged. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Raises ErrorsChanged. Ready to use.</summary>
    protected void RaiseErrorsChanged(string propertyName)
        => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}
