using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex021_
public partial class Ex021_RadioGroupBinding : UserControl
{
    public Ex021_RadioGroupBinding()
    {
        InitializeComponent();
        throw new NotImplementedException(
            "TODO: Ex021 - bind AlphaRadio/BetaRadio, one GroupName, through " +
            "Ex021_EnumMatchConverter to Selected, Mode=TwoWay");
    }
}

/// <summary>Given. Do not change.</summary>
public enum Ex021_Choice
{
    Alpha,
    Beta,
}

/// <summary>Given. Do not change.</summary>
public class Ex021_RadioGroupBindingViewModel : ReactiveObject
{
    private Ex021_Choice _selected = Ex021_Choice.Alpha;
    public Ex021_Choice Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
}

// This converter is part of the exercise's deliverable and is tested both
// directly (unit tests against Convert/ConvertBack alone) and through the UI -
// implementing only enough to satisfy the view does not satisfy the direct
// tests, and vice versa.
public class Ex021_EnumMatchConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex021 - true when value (the bound enum) equals parameter");

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex021 - parameter when value is true, BindingOperations.DoNothing " +
            "when value is false (an unchecking RadioButton must not overwrite the " +
            "selection another one in the group just made) - return the DoNothing " +
            "singleton exactly, not null and not a default enum member");
}
