// Exercise 010 - Dependent property fan-out (beginner). REFERENCE SOLUTION.
// Goal:   Handle the shape almost every real view model eventually needs - one field
//         backing several displayed properties - without dropping a notification for
//         any of them.
// Drills: raising PropertyChanged for more than one property name from a single
//         field's setter, in a fixed order, and only when the field actually changed.

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex010_TemperatureViewModel : INotifyPropertyChanged
{
    private double _celsius = 0.0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raw temperature in Celsius. Everything else on this class is computed
    /// from this one field.</summary>
    public double Celsius
    {
        get => _celsius;
        set
        {
            if (value == _celsius)
            {
                return;
            }

            _celsius = value;

            RaisePropertyChanged(nameof(Celsius));
            RaisePropertyChanged(nameof(Fahrenheit));
            RaisePropertyChanged(nameof(IsFreezing));
        }
    }

    /// <summary>Computed from <see cref="Celsius"/>.</summary>
    public double Fahrenheit => _celsius * 9.0 / 5.0 + 32.0;

    /// <summary>Computed from <see cref="Celsius"/>; true at or below freezing.</summary>
    public bool IsFreezing => _celsius <= 0.0;

    /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
    protected void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
