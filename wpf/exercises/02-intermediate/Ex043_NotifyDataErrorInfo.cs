// Exercise 043 - INotifyDataErrorInfo (intermediate).
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
// Passes: dotnet test --filter FullyQualifiedName~Ex043_

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
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// True while any property currently has at least one recorded error.
    /// </summary>
    public bool HasErrors
        => throw new NotImplementedException("TODO: Ex043 - true when the error store SetErrors maintains (add a private field, e.g. Dictionary<string, List<string>>) currently holds at least one property with at least one error - derive this from that SAME store, do not track a separate bool that SetErrors would also have to keep in sync");

    /// <summary>
    /// Returns the errors currently recorded for <paramref name="propertyName"/> - empty for
    /// none, and empty for a null/empty propertyName (this row does not model entity-level
    /// errors).
    /// </summary>
    public IEnumerable GetErrors(string? propertyName)
        => throw new NotImplementedException("TODO: Ex043 - return the errors recorded for propertyName from the same store HasErrors reads, or an empty sequence when propertyName is null/empty or nothing is recorded for it");

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
        => throw new NotImplementedException("TODO: Ex043 - store a copy of errors for propertyName in your error store (remove the entry entirely when errors is empty); if the stored set for propertyName actually changed, call RaiseErrorsChanged(propertyName); if HasErrors' value changed as a result of this call, call RaisePropertyChanged(nameof(HasErrors))");

    /// <summary>Raises PropertyChanged. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>Raises ErrorsChanged. Ready to use.</summary>
    protected void RaiseErrorsChanged(string propertyName)
        => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}
