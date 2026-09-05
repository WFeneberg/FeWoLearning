// Exercise 014 - StringFormat, FallbackValue and TargetNullValue (beginner).
// Goal:   Build the "invoice amount" label every real screen needs: formatted when
//         there is a value, a distinct message when there is genuinely no invoice at
//         all, and a different one when there is an invoice but its amount is not
//         filled in yet.
// Drills: Binding.StringFormat, Binding.FallbackValue (shown when the binding cannot
//         produce a value at all - no source, or a bad path) and Binding.TargetNullValue
//         (shown when the binding DID resolve, and the resolved value itself is null) -
//         two different failure shapes that a real Binding tells apart.
// Passes: dotnet test --filter FullyQualifiedName~Ex014_
//
// A note on culture, since this is the one row in the batch where it matters: a
// Binding takes its format culture from Binding.ConverterCulture, falling back to the
// bound element's Language property - never from Thread.CurrentCulture, no matter
// what the machine's OS locale is set to. The test fixes target.Language explicitly
// before calling Bind, so "{0:C}"'s currency formatting is deterministic on every
// machine this runs on. That pin is a given for this row, not something Bind needs to
// set up - do not add ConverterCulture here, that is row 069's subject.

using System.ComponentModel;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex014_StringFormatAndFallbacks
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="source"/>'s Amount.
    /// </summary>
    /// <param name="source">
    /// May be <see langword="null"/> itself - representing "no invoice selected at
    /// all", as opposed to an invoice whose Amount is null.
    /// </param>
    public static void Bind(TextBlock target, Ex014_InvoiceSource? source)
    {
        // TODO: put `source` in target.DataContext (even when it is null - that is
        // what makes FallbackValue observable below), then call target.SetBinding for
        // TextBlock.TextProperty with a Binding that has
        //   - Path "Amount" (use nameof, not a string literal),
        //   - StringFormat "{0:C}",
        //   - FallbackValue "no invoice" - shown when the binding cannot resolve a
        //     value at all (DataContext itself is null, here),
        //   - TargetNullValue "no amount yet" - shown when the binding DID resolve,
        //     and the resolved Amount itself is null.
        throw new NotImplementedException("TODO: Ex014 - bind Amount with StringFormat, FallbackValue and TargetNullValue");
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
public sealed class Ex014_InvoiceSource : INotifyPropertyChanged
{
    private decimal? _amount;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Null means "invoice exists, amount not entered yet" - distinct from
    /// <paramref name="source"/> itself being null in <see cref="Ex014_StringFormatAndFallbacks.Bind"/>.
    /// </summary>
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
