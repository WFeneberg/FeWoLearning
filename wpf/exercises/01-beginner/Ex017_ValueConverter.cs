// Exercise 017 - Value converter (beginner).
// Goal:   Translate a raw model value into a display value and back, and handle input
//         a converter genuinely cannot translate without throwing - that is what
//         DependencyProperty.UnsetValue is for.
// Drills: IValueConverter.Convert/ConvertBack, and DependencyProperty.UnsetValue as the
//         signal "this input has no valid translation" - in either direction, WPF
//         treats it as "do not push a value" rather than an exception.
// Passes: dotnet test --filter FullyQualifiedName~Ex017_

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
        // TODO: if value is an int equal to 1, 2 or 3, return "Low", "Medium" or
        // "High" respectively. For anything else (wrong type, or an int outside
        // 1-3), return DependencyProperty.UnsetValue - there is no label for it, and
        // that is not the same failure as an exception.
        throw new NotImplementedException("TODO: Ex017 - convert a known priority code to its label, else DependencyProperty.UnsetValue");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // TODO: if value is the string "Low", "Medium" or "High" (ordinal, exact
        // case), return 1, 2 or 3 respectively. For anything else (including a typo
        // or partial word), return DependencyProperty.UnsetValue - the binding must
        // not push garbage back into the source.
        throw new NotImplementedException("TODO: Ex017 - convert a known label back to its priority code, else DependencyProperty.UnsetValue");
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
        // TODO: target.DataContext = source, then target.SetBinding for
        // TextBox.TextProperty with a Binding that has
        //   - Path nameof(Ex017_PrioritySource.PriorityCode),
        //   - Mode BindingMode.TwoWay,
        //   - Converter = new Ex017_PriorityConverter(),
        //   - UpdateSourceTrigger UpdateSourceTrigger.PropertyChanged (so editing
        //     Text pushes immediately - no focus dance needed for this row).
        throw new NotImplementedException("TODO: Ex017 - bind PriorityCode two-way through Ex017_PriorityConverter");
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
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
