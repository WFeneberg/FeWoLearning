using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Exercises.Expert;

// Exercise 094 — Avalonia-style MVVM view-model (reference solution).
// A minimal, dependency-free RelayCommand plus a view-model that wires it up:
// executing the command mutates state, notifies bound properties, and
// re-evaluates its own CanExecute, exactly like a real Avalonia/WPF/MAUI
// MVVM binding chain (View -> ICommand -> ViewModel -> INotifyPropertyChanged).
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

    private readonly RelayCommand _incrementCommand;
    private int _count;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvaloniaMvvmViewModel()
    {
        _incrementCommand = new RelayCommand(_ => Increment(), _ => CanIncrement);
    }

    public int Count
    {
        get => _count;
        private set
        {
            if (_count == value)
                return;

            var wasCanIncrement = CanIncrement;
            _count = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanIncrement));

            if (wasCanIncrement != CanIncrement)
                _incrementCommand.RaiseCanExecuteChanged();
        }
    }

    public bool CanIncrement => _count < MaxCount;

    public ICommand IncrementCommand => _incrementCommand;

    private void Increment()
    {
        if (!CanIncrement)
            return;

        Count++;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // Minimal, framework-agnostic RelayCommand — the same shape used by
    // Avalonia/WPF/MAUI/CommunityToolkit.Mvvm view-models.
    private sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
