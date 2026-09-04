using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 009 - ReactiveObjectBasics (beginner).
/// Goal:   Get the whole of Ex008 for one line, and get PropertyChanging for free.
/// Drills: ReactiveObject, RaiseAndSetIfChanged, PropertyChanging ordering.
/// Passes: dotnet test --filter FullyQualifiedName~Ex009_
public class Ex009_ReactiveObjectBasics : ReactiveObject
{
    private int _count;

    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }
}
