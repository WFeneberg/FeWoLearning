// Exercise 033 - Clear Value And Default (beginner).
// Goal:   Undo a local value and see what the property falls back to.
// Drills: ClearValue removing only the local value, the fallback to a style setter and then
//         to the registered default, and per-instance isolation of the property store.
// Passes: dotnet test --filter FullyQualifiedName~Ex033_
//
// Setting a property back to "the value it had before" by hand is a bug waiting to happen:
// the previous value may have come from a style that has since changed. ClearValue removes
// the local value and lets the property system answer again.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex033_ClearValueAndDefault : DependencyObject
{
    /// <summary>Registered with a default of 5.</summary>
    public static readonly DependencyProperty QuantityProperty =
        DependencyProperty.Register(
            nameof(Quantity),
            typeof(int),
            typeof(Ex033_ClearValueAndDefault),
            new PropertyMetadata(5));

    public int Quantity
    {
        get => (int)GetValue(QuantityProperty);
        set => SetValue(QuantityProperty, value);
    }

    /// <summary>
    /// Whether <see cref="Quantity"/> currently has a value set on this instance, as
    /// opposed to reporting the registered default.
    /// </summary>
    public bool HasLocalQuantity =>
        throw new NotImplementedException("TODO: Ex033 - is there a local value?");

    /// <summary>
    /// Forgets any locally set <see cref="Quantity"/>. Calling it on an instance that never
    /// had one is a no-op, not an error.
    /// </summary>
    public void ResetQuantity() =>
        throw new NotImplementedException("TODO: Ex033 - drop the local value");
}
