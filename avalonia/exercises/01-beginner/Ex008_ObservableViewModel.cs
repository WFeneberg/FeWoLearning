using System.ComponentModel;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 008 - ObservableViewModel (beginner).
/// Goal:   Implement INotifyPropertyChanged by hand, exactly once in this track.
/// Drills: INotifyPropertyChanged, change-only notification.
/// Passes: dotnet test --filter FullyQualifiedName~Ex008_
public class Ex008_ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _count;

    /// <summary>
    /// TODO: return the stored value, and on set store it and raise PropertyChanged
    /// with the property's name - but ONLY when the incoming value actually differs
    /// from the current one. Assigning the same value again must raise nothing.
    /// </summary>
    public int Count
    {
        get => throw new NotImplementedException(
            "TODO: Ex008 - return the backing field");
        set => throw new NotImplementedException(
            "TODO: Ex008 - store and raise PropertyChanged only on a real change");
    }
}
