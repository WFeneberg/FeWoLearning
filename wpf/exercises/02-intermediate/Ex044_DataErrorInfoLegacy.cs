// Exercise 044 - IDataErrorInfo, the legacy validation interface (intermediate).
// Goal:   Wire up WPF's original, pre-.NET-4.5 validation interface, and measure the one
//         thing that makes it a genuinely distinct row rather than a footnote on row 043:
//         Binding.ValidatesOnDataErrors defaults to FALSE, the opposite of row 043's
//         ValidatesOnNotifyDataErrors. Measured directly on this harness: a TextBox bound to
//         an IDataErrorInfo source, with the flag left unset, shows Validation.GetHasError as
//         false even while the source's own indexer would report an error for that property -
//         the binding never asks. Setting ValidatesOnDataErrors = true is what turns the
//         indexer's answer into a real validation error on the target.
// Drills: IDataErrorInfo (the this[string] indexer), and Binding.ValidatesOnDataErrors -
//         which, unlike row 043's interface, must be set explicitly or the whole mechanism is
//         silently inert.
// Passes: dotnet test --filter FullyQualifiedName~Ex044_

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ships only the validation plumbing - no concrete validated view model here, for the same
/// reason row 043's base ships none: a "ready to use" validating view model would let the
/// indexer wiring below go untested through it instead of through the interface itself.
/// </summary>
public abstract class Ex044_LegacyValidatingViewModelBase : INotifyPropertyChanged, IDataErrorInfo
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Entity-level error - out of scope for this row. Ready to use.</summary>
    string IDataErrorInfo.Error => string.Empty;

    /// <summary>
    /// The legacy per-property indexer a Binding queries when ValidatesOnDataErrors is true.
    /// </summary>
    string IDataErrorInfo.this[string columnName]
        => throw new NotImplementedException("TODO: Ex044 - forward to GetError(columnName), the protected hook below a concrete validating view model overrides; treat a null result as no error (an empty string)");

    /// <summary>
    /// Override to supply the message for <paramref name="propertyName"/>, or null/empty
    /// when it is currently valid. This is what a concrete validating view model implements.
    /// </summary>
    protected abstract string? GetError(string propertyName);

    /// <summary>Raises PropertyChanged. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public static class Ex044_DataErrorInfoLegacy
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text two-way to the property at
    /// <paramref name="propertyPath"/> on <paramref name="source"/>, PropertyChanged trigger,
    /// with the legacy IDataErrorInfo validation flag explicitly turned ON - the one line
    /// this row's Concepts cell names, because unlike row 043's interface, it defaults off.
    /// </summary>
    public static void BindWithLegacyValidation(TextBox target, object source, string propertyPath)
        => throw new NotImplementedException("TODO: Ex044 - target.DataContext = source, then target.SetBinding(TextBox.TextProperty, new Binding(propertyPath) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, ValidatesOnDataErrors = true })");
}
