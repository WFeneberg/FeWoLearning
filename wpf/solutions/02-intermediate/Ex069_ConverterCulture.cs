// Exercise 069 - ConverterCulture vs Thread.CurrentUICulture. REFERENCE SOLUTION.
// Goal:   Row 018 already showed a Binding formatting through an explicit ConverterCulture. This
//         row is the CONTRAST that makes ConverterCulture worth remembering at all: a Binding's
//         format culture comes from Binding.ConverterCulture, falling back to the bound element's
//         Language (a hard-coded "en-US" default) - and NEVER from Thread.CurrentCulture or
//         Thread.CurrentUICulture, no matter what the OS locale says. A developer who "fixes"
//         wrong-looking currency formatting by setting the ambient thread culture is fixing
//         nothing a Binding will ever look at.
// Drills: Binding.ConverterCulture actually overriding a DISAGREEING thread culture, not merely
//         agreeing with it by coincidence.

using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex069_ConverterCulture
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="source"/>'s Amount, formatted as
    /// currency (StringFormat "{0:C}") using <paramref name="culture"/> as the binding's
    /// ConverterCulture - the only thing that actually controls a Binding's format culture, and
    /// unrelated to whatever Thread.CurrentCulture/CurrentUICulture happen to be set to at the
    /// time.
    /// </summary>
    public static void BindWithExplicitCulture(TextBlock target, Ex069_AmountSource source, CultureInfo culture)
    {
        target.DataContext = source;
        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex069_AmountSource.Amount))
        {
            StringFormat = "{0:C}",
            ConverterCulture = culture,
        });
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
public sealed class Ex069_AmountSource : INotifyPropertyChanged
{
    private decimal _amount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public decimal Amount
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
