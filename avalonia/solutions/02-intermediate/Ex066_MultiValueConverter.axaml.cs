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
    }
}

public class Ex066_FullNameConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3 ||
            values[0] is not string first ||
            values[1] is not string last ||
            values[2] is not bool isFellow)
        {
            return string.Empty;
        }

        return isFellow ? $"{first} {last} (FRS)" : $"{first} {last}";
    }
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
