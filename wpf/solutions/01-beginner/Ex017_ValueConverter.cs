// Exercise 017 - Value converter (beginner). REFERENCE SOLUTION.
// Goal:   Translate a raw model value into a display value and back, and handle input
//         a converter genuinely cannot translate without throwing - that is what
//         DependencyProperty.UnsetValue is for.
// Drills: IValueConverter.Convert/ConvertBack, and DependencyProperty.UnsetValue as the
//         signal "this input has no valid translation" - in either direction, WPF
//         treats it as "do not push a value" rather than an exception.

using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>Converts between a numeric priority code and its display label.</summary>
public sealed class Ex017_PriorityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int code)
        {
            return code switch
            {
                1 => "Low",
                2 => "Medium",
                3 => "High",
                _ => DependencyProperty.UnsetValue,
            };
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string label)
        {
            return label switch
            {
                "Low" => 1,
                "Medium" => 2,
                "High" => 3,
                _ => DependencyProperty.UnsetValue,
            };
        }

        return DependencyProperty.UnsetValue;
    }
}

public static class Ex017_ValueConverter
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text two-way to <paramref name="source"/>'s
    /// PriorityCode, through <see cref="Ex017_PriorityConverter"/>.
    /// </summary>
    public static void Bind(TextBox target, Ex017_PrioritySource source)
    {
        target.DataContext = source;

        target.SetBinding(TextBox.TextProperty, new Binding(nameof(Ex017_PrioritySource.PriorityCode))
        {
            Mode = BindingMode.TwoWay,
            Converter = new Ex017_PriorityConverter(),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
    }
}

/// <summary>The model behind the label.</summary>
public sealed class Ex017_PrioritySource : INotifyPropertyChanged
{
    private int _priorityCode;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int PriorityCode
    {
        get => _priorityCode;
        set
        {
            if (_priorityCode == value)
            {
                return;
            }

            _priorityCode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PriorityCode)));
        }
    }
}
