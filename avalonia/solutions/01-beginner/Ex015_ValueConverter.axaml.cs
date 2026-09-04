using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex015_
public partial class Ex015_ValueConverter : UserControl
{
    public Ex015_ValueConverter() => InitializeComponent();
}

public class Ex015_ValueConverterViewModel : ReactiveObject
{
    private double _celsius;
    public double Celsius
    {
        get => _celsius;
        set => this.RaiseAndSetIfChanged(ref _celsius, value);
    }
}

public class Ex015_CelsiusToFahrenheitConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var celsius = (double)value!;
        return (celsius * 9 / 5 + 32).ToString(CultureInfo.InvariantCulture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text &&
            double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var fahrenheit))
        {
            return (fahrenheit - 32) * 5 / 9;
        }

        return BindingOperations.DoNothing;
    }
}
