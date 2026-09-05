// Exercise 014 - StringFormat, FallbackValue and TargetNullValue (beginner). REFERENCE SOLUTION.
// Goal:   Build the "invoice amount" label every real screen needs: formatted when
//         there is a value, a distinct message when there is genuinely no invoice at
//         all, and a different one when there is an invoice but its amount is not
//         filled in yet.
// Drills: Binding.StringFormat, Binding.FallbackValue (shown when the binding cannot
//         produce a value at all - no source, or a bad path) and Binding.TargetNullValue
//         (shown when the binding DID resolve, and the resolved value itself is null) -
//         two different failure shapes that a real Binding tells apart.
//
// A note on culture: a Binding takes its format culture from Binding.ConverterCulture,
// falling back to the bound element's Language property - never from
// Thread.CurrentCulture. This solution does not touch either; the test pins
// target.Language before calling Bind, which is enough to make "{0:C}" deterministic
// regardless of the machine's OS locale. ConverterCulture is row 069's subject, not
// this one's.

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex014_StringFormatAndFallbacks
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="source"/>'s Amount.
    /// </summary>
    public static void Bind(TextBlock target, Ex014_InvoiceSource? source)
    {
        target.DataContext = source;

        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex014_InvoiceSource.Amount))
        {
            StringFormat = "{0:C}",

            // Binding cannot resolve a value at all - here, because DataContext
            // itself is null.
            FallbackValue = "no invoice",

            // Binding resolved fine; the value it found was null.
            TargetNullValue = "no amount yet",
        });
    }
}

/// <summary>The model behind the label.</summary>
public sealed class Ex014_InvoiceSource : INotifyPropertyChanged
{
    private decimal? _amount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public decimal? Amount
    {
        get => _amount;
        set
        {
            if (_amount == value)
            {
                return;
            }

            _amount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
        }
    }
}
