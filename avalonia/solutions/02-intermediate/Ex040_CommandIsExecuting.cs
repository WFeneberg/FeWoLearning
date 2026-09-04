using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 040 - CommandIsExecuting (intermediate).
/// Goal:   Surface a command's IsExecuting as a bindable IsBusy property, so a
///         concurrent invocation can be gated on it.
/// Drills: IsExecuting gating concurrent invocation.
/// Passes: dotnet test --filter FullyQualifiedName~Ex040_
public class Ex040_CommandIsExecutingViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    public ReactiveCommand<RxVoid, string> RunCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _isBusy;
    public bool IsBusy => _isBusy.Value;

    public Ex040_CommandIsExecutingViewModel(Func<Task<string>> work)
    {
        _work = work;
        RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
        _isBusy = RunCommand.IsExecuting.ToProperty(this, x => x.IsBusy);
    }
}
