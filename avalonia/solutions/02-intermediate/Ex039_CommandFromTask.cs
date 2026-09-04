using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 039 - CommandFromTask (intermediate).
/// Goal:   Wrap a given async unit of work in a ReactiveCommand and surface its
///         awaited result on the view model.
/// Drills: ReactiveCommand.CreateFromTask, awaiting a result.
/// Passes: dotnet test --filter FullyQualifiedName~Ex039_
public class Ex039_CommandFromTaskViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string _result = string.Empty;
    public string Result { get => _result; private set => this.RaiseAndSetIfChanged(ref _result, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    public Ex039_CommandFromTaskViewModel(Func<Task<string>> work)
    {
        _work = work;
        FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
        FetchCommand.Subscribe(value => Result = value);
    }
}
