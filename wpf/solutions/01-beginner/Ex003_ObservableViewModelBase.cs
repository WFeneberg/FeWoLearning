// Exercise 003 - Observable view-model base (beginner). REFERENCE SOLUTION.
// Goal:   Build the SetProperty helper every hand-written MVVM migration ends up with,
//         so a view model stops repeating four lines per property.
// Drills: INotifyPropertyChanged, EqualityComparer<T>.Default, [CallerMemberName] for
//         the property name, and raising the event only on a real change.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public abstract class Ex003_ObservableViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Assigns <paramref name="value"/> to <paramref name="field"/> if it actually
    /// changed, raises <see cref="PropertyChanged"/> for the caller's property name, and
    /// reports whether anything changed.
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // EqualityComparer<T>.Default, not ==: for an unconstrained T the operator would
        // compile to reference equality, so two equal strings would count as a change.
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>A small view model whose properties go through the base.</summary>
public sealed class Ex003_MeterViewModel : Ex003_ObservableViewModelBase
{
    private double _reading;
    private string _label = string.Empty;

    public double Reading
    {
        get => _reading;
        set => SetProperty(ref _reading, value);
    }

    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }
}
