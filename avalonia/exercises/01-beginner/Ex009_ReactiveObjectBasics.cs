using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Beginner;

/// Exercise 009 - ReactiveObjectBasics (beginner).
/// Goal:   Get the whole of Ex008 for one line, and get PropertyChanging for free.
/// Drills: ReactiveObject, RaiseAndSetIfChanged, PropertyChanging ordering.
/// Passes: dotnet test --filter FullyQualifiedName~Ex009_
public class Ex009_ReactiveObjectBasics : ReactiveObject
{
    private int _count;

    /// <summary>
    /// TODO: implement this the ReactiveUI way, with a single call in the setter.
    /// Besides the change-only PropertyChanged of Ex008, the tests also require
    /// PropertyChanging to be raised BEFORE the backing field is updated - which
    /// you get for free from the right helper, and cannot get from a hand-written
    /// OnPropertyChanged.
    /// </summary>
    public int Count
    {
        get => throw new NotImplementedException(
            "TODO: Ex009 - return the backing field");
        set => throw new NotImplementedException(
            "TODO: Ex009 - use the ReactiveObject helper that raises both events");
    }
}
