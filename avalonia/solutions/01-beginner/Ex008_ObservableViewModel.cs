using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 008 - ObservableViewModel (beginner).
/// Goal:   Implement INotifyPropertyChanged by hand, exactly once in this track.
/// Drills: INotifyPropertyChanged, change-only notification.
/// Passes: dotnet test --filter FullyQualifiedName~Ex008_
public class Ex008_ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _count;

    public int Count
    {
        get => _count;
        set
        {
            if (_count == value)
                return;

            _count = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
