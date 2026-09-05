// Exercise 019 - MultiBinding (beginner).
// Goal:   Combine two independent source properties into one displayed string, live -
//         something a single Binding and StringFormat cannot do, because there is only
//         one path to format there.
// Drills: MultiBinding.Bindings (a list of ordinary Bindings, each read off the same
//         DataContext by default) and IMultiValueConverter.Convert, which receives
//         all of them at once as an object[], in declaration order.
// Passes: dotnet test --filter FullyQualifiedName~Ex019_

using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>Combines a first and last name into one displayed full name.</summary>
public sealed class Ex019_FullNameConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // TODO: values[0] is FirstName, values[1] is LastName (that is the order the
        // Bindings are added in below) - both plain strings. Return
        // $"{values[0]} {values[1]}".
        throw new NotImplementedException("TODO: Ex019 - combine values[0] (FirstName) and values[1] (LastName) into one string");
    }

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
        // TODO: target.DataContext = source, then build a
        // System.Windows.Data.MultiBinding whose Converter is
        // new Ex019_FullNameConverter() and whose Bindings contains, in this order,
        //   - new Binding(nameof(Ex019_PersonNameSource.FirstName)),
        //   - new Binding(nameof(Ex019_PersonNameSource.LastName)),
        // then target.SetBinding(TextBlock.TextProperty, multiBinding).
        throw new NotImplementedException("TODO: Ex019 - combine FirstName and LastName through a MultiBinding");
    }
}

/// <summary>The model behind the label. Ready to use.</summary>
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
