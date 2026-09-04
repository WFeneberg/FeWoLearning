// Exercise 010 - Dependent property fan-out (beginner).
// Goal:   Handle the shape almost every real view model eventually needs - one field
//         backing several displayed properties - without dropping a notification for
//         any of them.
// Drills: raising PropertyChanged for more than one property name from a single
//         field's setter, in a fixed order, and only when the field actually changed.
// Passes: dotnet test --filter FullyQualifiedName~Ex010_

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex010_TemperatureViewModel : INotifyPropertyChanged
{
    // Explicit "= 0.0" initializer, matching the registered default: the setter below
    // throws before it ever assigns this field, and without an initializer that makes
    // the compiler warn CS0649 ("field is never assigned"). Same value it would have had
    // anyway - this is a warning workaround, not part of the exercise.
    private double _celsius = 0.0;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raw temperature in Celsius. Everything else on this class is computed
    /// from this one field.</summary>
    public double Celsius
    {
        get => _celsius;
        set
        {
            // TODO: if value equals the current _celsius, do nothing at all - no field
            // write, no events. Otherwise assign _celsius, then raise PropertyChanged
            // for "Celsius", "Fahrenheit" and "IsFreezing", in that exact order.
            // Fahrenheit and IsFreezing have no field of their own to compare against -
            // they are computed from Celsius, so every real change to Celsius is
            // automatically a change to both of them too.
            throw new NotImplementedException("TODO: Ex010 - assign Celsius and fan its change out to Fahrenheit and IsFreezing");
        }
    }

    /// <summary>Computed from <see cref="Celsius"/>. Ready to use.</summary>
    public double Fahrenheit => _celsius * 9.0 / 5.0 + 32.0;

    /// <summary>Computed from <see cref="Celsius"/>; true at or below freezing. Ready to use.</summary>
    public bool IsFreezing => _celsius <= 0.0;

    /// <summary>Raises <see cref="PropertyChanged"/>. Ready to use.</summary>
    protected void RaisePropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
