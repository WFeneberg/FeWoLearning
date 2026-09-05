using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 042 - CommandCancellation (intermediate).
/// Goal:   Wrap a given cancellable async unit of work in a ReactiveCommand, so
///         disposing an in-flight execution's subscription genuinely cancels it.
/// Drills: CancellationToken in CreateFromTask.
/// Passes: dotnet test --filter FullyQualifiedName~Ex042_
public class Ex042_CommandCancellationViewModel : ReactiveObject
{
    private readonly Func<CancellationToken, Task<string>> _work;

    public ReactiveCommand<RxVoid, string> RunCommand { get; }

    public Ex042_CommandCancellationViewModel(Func<CancellationToken, Task<string>> work)
    {
        _work = work;
        RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    }
}
