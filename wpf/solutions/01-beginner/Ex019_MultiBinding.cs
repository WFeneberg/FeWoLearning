// Exercise 019 - MultiBinding (beginner). REFERENCE SOLUTION.
// Goal:   Combine two independent source properties into one displayed string, live -
//         something a single Binding and StringFormat cannot do, because there is only
//         one path to format there.
// Drills: MultiBinding.Bindings (a list of ordinary Bindings, each read off the same
//         DataContext by default) and IMultiValueConverter.Convert, which receives
//         all of them at once as an object[], in declaration order.

using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>Combines a first and last name into one displayed full name.</summary>
public sealed class Ex019_FullNameConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        => $"{values[0]} {values[1]}";

    /// <summary>Not used - this row's binding is one-way, display only.</summary>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException("Ex019's binding is one-way; ConvertBack is intentionally unused.");
}

public static class Ex019_MultiBinding
{
    /// <summary>
    /// Binds <paramref name="target"/>'s Text to <paramref name="source"/>'s
    /// FirstName and LastName combined through <see cref="Ex019_FullNameConverter"/>.
    /// </summary>
    public static void Bind(TextBlock target, Ex019_PersonNameSource source)
    {
        target.DataContext = source;

        var multiBinding = new MultiBinding
        {
            Converter = new Ex019_FullNameConverter(),
        };
        multiBinding.Bindings.Add(new Binding(nameof(Ex019_PersonNameSource.FirstName)));
        multiBinding.Bindings.Add(new Binding(nameof(Ex019_PersonNameSource.LastName)));

        target.SetBinding(TextBlock.TextProperty, multiBinding);
    }
}

/// <summary>The model behind the label.</summary>
public sealed class Ex019_PersonNameSource : INotifyPropertyChanged
{
    private string _firstName = "";
    private string _lastName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName == value)
            {
                return;
            }

            _firstName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (_lastName == value)
            {
                return;
            }

            _lastName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
        }
    }
}
