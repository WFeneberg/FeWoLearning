// Exercise 018 - ConverterParameter and ConverterCulture (beginner). REFERENCE SOLUTION.
// Goal:   Feed a converter something beyond the bound value itself: a fixed,
//         per-binding parameter (unrelated to any source property) and an explicit
//         culture to format numbers in - the two knobs a Binding hands a converter in
//         addition to the value.
// Drills: Binding.ConverterParameter (read via IValueConverter's own `parameter`
//         argument, never through the source object) and Binding.ConverterCulture.
//
// A note on culture: a Binding takes its format culture from Binding.ConverterCulture,
// falling back to the bound element's Language property (a hard-coded "en-US"
// default) - never Thread.CurrentCulture. Bind sets ConverterCulture explicitly, so
// the formatted output is deterministic regardless of the machine's OS locale.
// ConverterCulture vs Thread.CurrentUICulture is row 069's subject, not this one's.

using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>Multiplies a unit price by a quantity and formats the total as currency.</summary>
public sealed class Ex018_TotalPriceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var unitPrice = (decimal)value;
        var quantity = System.Convert.ToInt32(parameter);

        return (unitPrice * quantity).ToString("C", culture);
    }

    /// <summary>Not used - this row's binding is one-way, display only.</summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException("Ex018's binding is one-way; ConvertBack is intentionally unused.");
}

public static class Ex018_ConverterParameterAndCulture
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="source"/>'s
    /// UnitPrice, multiplied by <paramref name="quantity"/> and formatted in
    /// <paramref name="culture"/>.
    /// </summary>
    public static void Bind(TextBlock target, Ex018_LineItemSource source, int quantity, CultureInfo culture)
    {
        target.DataContext = source;

        target.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex018_LineItemSource.UnitPrice))
        {
            Converter = new Ex018_TotalPriceConverter(),
            ConverterParameter = quantity,
            ConverterCulture = culture,
        });
    }
}

/// <summary>The model behind the label.</summary>
public sealed class Ex018_LineItemSource : INotifyPropertyChanged
{
    private decimal _unitPrice;

    public event PropertyChangedEventHandler? PropertyChanged;

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice == value)
            {
                return;
            }

            _unitPrice = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnitPrice)));
        }
    }
}
