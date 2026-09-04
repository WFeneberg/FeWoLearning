using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 039 - CommandFromTask (intermediate).
/// Goal:   Wrap a given async unit of work in a ReactiveCommand and surface its
///         awaited result on the view model.
/// Drills: ReactiveCommand.CreateFromTask, awaiting a result.
///
/// Measured on this machine against ReactiveUI 24.1.0: ReactiveCommand.CreateFromTask(fn)
/// - the overload with NO ISequencer argument - leaves the command's
/// IsExecuting/CanExecute state machinery dead (ex040 proves that decisively).
/// Separately measured for THIS exercise: Sequencer.Default (a thread pool) also
/// marshals the Result-storing subscription independently of the awaited task's
/// own completion, so "await command.Execute()" can return before Result has
/// actually been set - a genuine race, not just a style difference. Use
/// Sequencer.CurrentThread specifically (from ReactiveUI.Primitives.Concurrency)
/// as the second argument.
/// Passes: dotnet test --filter FullyQualifiedName~Ex039_
public class Ex039_CommandFromTaskViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string _result = string.Empty;
    public string Result { get => _result; private set => this.RaiseAndSetIfChanged(ref _result, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    /// <summary>
    /// TODO:
    ///   FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    ///   FetchCommand.Subscribe(value => Result = value);
    /// _work is given by the test so it can gate completion on a
    /// TaskCompletionSource - do not call it synchronously (no .Result, no
    /// .Wait(), no .GetAwaiter().GetResult()) and do not fabricate a result:
    /// the tests await the command and check the exact awaited value.
    /// </summary>
    public Ex039_CommandFromTaskViewModel(Func<Task<string>> work)
    {
        _work = work;
        throw new NotImplementedException(
            "TODO: Ex039 - FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread), " +
            "and pipe each result into Result");
    }
}
