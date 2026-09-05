using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex066_
public partial class Ex066_MultiValueConverter : UserControl
{
    public Ex066_MultiValueConverter()
    {
        InitializeComponent();
        DataContext = new Ex066_MultiValueConverterViewModel();
        throw new NotImplementedException(
            "TODO: Ex066 - register Ex066_FullNameConverter as a resource and give a " +
            "TextBlock named FullName a MultiBinding over First, Last and IsFellow");
    }
}

/// <summary>
/// The converter the view drives. Avalonia's IMultiValueConverter has only a
/// Convert - there is no ConvertBack, because a single target value cannot be
/// pushed back into several sources.
/// </summary>
public class Ex066_FullNameConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex066 - values arrives positionally as [First, Last, IsFellow]. " +
            "Return \"First Last\", with \" (FRS)\" appended when IsFellow is true. " +
            "Any value that is not the expected type must yield an empty string " +
            "rather than throwing. That case is guaranteed, not hypothetical: " +
            "measured, Avalonia calls a MultiBinding's converter once per binding " +
            "as each settles, and the first call carries three UnsetValues");
}

/// <summary>Given. Do not change.</summary>
public class Ex066_MultiValueConverterViewModel : ReactiveObject
{
    private string _first = "Ada";
    private string _last = "Lovelace";
    private bool _isFellow;

    public string First
    {
        get => _first;
        set => this.RaiseAndSetIfChanged(ref _first, value);
    }

    public string Last
    {
        get => _last;
        set => this.RaiseAndSetIfChanged(ref _last, value);
    }

    public bool IsFellow
    {
        get => _isFellow;
        set => this.RaiseAndSetIfChanged(ref _isFellow, value);
    }
}
