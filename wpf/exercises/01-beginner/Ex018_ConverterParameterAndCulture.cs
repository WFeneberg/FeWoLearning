// Exercise 018 - ConverterParameter and ConverterCulture (beginner).
// Goal:   Feed a converter something beyond the bound value itself: a fixed,
//         per-binding parameter (unrelated to any source property) and an explicit
//         culture to format numbers in - the two knobs a Binding hands a converter in
//         addition to the value.
// Drills: Binding.ConverterParameter (read via IValueConverter's own `parameter`
//         argument, never through the source object) and Binding.ConverterCulture.
//
// A note on culture, since this is the row that owns it: a Binding takes its format
// culture from Binding.ConverterCulture, falling back to the bound element's Language
// property, which defaults to a hard-coded "en-US" - never from Thread.CurrentCulture,
// no matter the OS locale. That means the only way to get a non-English format here is
// to set ConverterCulture explicitly, which is exactly what Bind below must do -
// contrast row 069, which is about the Thread.CurrentCulture pitfall this dodges.
// Passes: dotnet test --filter FullyQualifiedName~Ex018_

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
        // TODO: value is a decimal unit price. parameter is the quantity, boxed as the
        // int Bind put there - convert it with System.Convert.ToInt32(parameter)
        // anyway: that is the habit worth having, because in XAML ConverterParameter
        // always arrives as a string, and ToInt32 handles both. Return
        // (unitPrice * quantity).ToString("C", culture) - the "C" formats as currency,
        // and it is `culture` - the argument this method receives - that must do the
        // formatting, not CultureInfo.CurrentCulture or CultureInfo.InvariantCulture.
        throw new NotImplementedException("TODO: Ex018 - multiply by the ConverterParameter quantity and format as currency in the given culture");
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
        // TODO: target.DataContext = source, then target.SetBinding for
        // TextBlock.TextProperty with a Binding that has
        //   - Path nameof(Ex018_LineItemSource.UnitPrice),
        //   - Converter = new Ex018_TotalPriceConverter(),
        //   - ConverterParameter = quantity (boxes the int - do not stringify it),
        //   - ConverterCulture = culture.
        throw new NotImplementedException("TODO: Ex018 - bind UnitPrice through Ex018_TotalPriceConverter with ConverterParameter=quantity and ConverterCulture=culture");
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
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
