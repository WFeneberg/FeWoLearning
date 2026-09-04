using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

// Passes: dotnet test --filter FullyQualifiedName~Ex021_
public partial class Ex021_RadioGroupBinding : UserControl
{
    public Ex021_RadioGroupBinding() => InitializeComponent();
}

public enum Ex021_Choice
{
    Alpha,
    Beta,
}

public class Ex021_RadioGroupBindingViewModel : ReactiveObject
{
    private Ex021_Choice _selected = Ex021_Choice.Alpha;
    public Ex021_Choice Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }
}

public class Ex021_EnumMatchConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not null && value.Equals(parameter);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? parameter : BindingOperations.DoNothing;
}
