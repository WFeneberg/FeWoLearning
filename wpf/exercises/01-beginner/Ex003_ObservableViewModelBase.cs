// Exercise 003 - Observable view-model base (beginner).
// Goal:   Build the SetProperty helper every hand-written MVVM migration ends up with,
//         so a view model stops repeating four lines per property.
// Drills: INotifyPropertyChanged, EqualityComparer<T>.Default, [CallerMemberName] for
//         the property name, and raising the event only on a real change.
// Passes: dotnet test --filter FullyQualifiedName~Ex003_

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
        // TODO: compare field and value with EqualityComparer<T>.Default. If they are
        // equal, change nothing and return false. Otherwise assign, raise
        // PropertyChanged for propertyName, and return true.
        //
        // Do NOT default propertyName by hand - [CallerMemberName] is what supplies it,
        // and one of the tests checks that the compiler filled it in.
        throw new NotImplementedException("TODO: Ex003 - implement SetProperty");
    }

    /// <summary>Raises <see cref="PropertyChanged"/>. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>A small view model whose properties go through the base. Ready to use.</summary>
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
