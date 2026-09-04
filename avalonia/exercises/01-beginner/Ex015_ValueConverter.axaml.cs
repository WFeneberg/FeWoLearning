using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex015_
public partial class Ex015_ValueConverter : UserControl
{
    public Ex015_ValueConverter()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex015 - implement Ex015_CelsiusToFahrenheitConverter and bind " +
            "FahrenheitBox.Text through it, Mode=TwoWay, to Celsius");
    }
}

/// <summary>Given. Do not change.</summary>
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
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex015 - Celsius (double) -> Fahrenheit string, invariant culture");

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex015 - Fahrenheit string -> Celsius (double), invariant culture");
}
