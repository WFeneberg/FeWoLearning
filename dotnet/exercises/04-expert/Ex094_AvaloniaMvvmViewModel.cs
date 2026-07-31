using System.ComponentModel;

namespace FeWoLearning.Exercises.Expert;

// Exercise 094 — Avalonia-style MVVM view-model (expert).
// Goal:   Implement an INotifyPropertyChanged view-model whose IncrementCommand
//         (a RelayCommand-driven ICommand) mutates bound state, raises
//         PropertyChanged for every affected property name, and disables
//         itself (raising CanExecuteChanged) once a maximum is reached.
// Drills: INotifyPropertyChanged, ICommand/RelayCommand pattern, derived
//         ("CanIncrement") properties, command re-evaluation.
//
// This exercise is self-contained (no Avalonia/WPF package reference), so it
// declares its own minimal ICommand contract shaped exactly like the real
// System.Windows.Input.ICommand used by every XAML-based MVVM UI framework
// (Avalonia, WPF, MAUI, Uno, ...).
public interface ICommand
{
    event EventHandler? CanExecuteChanged;
    bool CanExecute(object? parameter);
    void Execute(object? parameter);
}

public sealed class AvaloniaMvvmViewModel : INotifyPropertyChanged
{
    public const int MaxCount = 5;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvaloniaMvvmViewModel() => throw new NotImplementedException();

    // Current counter value, starts at 0.
    public int Count => throw new NotImplementedException();

    // True while Count is below MaxCount.
    public bool CanIncrement => throw new NotImplementedException();

    // Executing this command increments Count by 1 (while CanIncrement is true),
    // raises PropertyChanged for both Count and CanIncrement, and raises the
    // command's CanExecuteChanged whenever CanIncrement's value flips.
    public ICommand IncrementCommand => throw new NotImplementedException();
}
