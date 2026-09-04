using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 040 - CommandIsExecuting (intermediate).
/// Goal:   Surface a command's IsExecuting as a bindable IsBusy property, so a
///         concurrent invocation can be gated on it.
/// Drills: IsExecuting gating concurrent invocation.
///
/// THE decisive measured fact for this whole batch: ReactiveCommand.CreateFromTask(fn)
/// - the overload with NO ISequencer argument - leaves the state machinery dead:
/// IsExecuting emits [false] only (true never fires) and CanExecute never goes
/// false while the task is in flight - measured with a subscriber waiting two
/// full seconds for a true that never arrived. Passing Sequencer.CurrentThread
/// (from ReactiveUI.Primitives.Concurrency) as the second argument is what makes
/// both fire, on the calling thread, so the sequence is deterministic:
/// [false, true, false], with CanExecute false in between. Sequencer.Default
/// (the implicit default, a thread pool) also eventually fires true/false, but
/// measured 4 times out of 5 to still read [false] at the exact instant this
/// exercise's tests check mid-flight state - not deterministic enough to test
/// against. Use Sequencer.CurrentThread specifically.
/// Passes: dotnet test --filter FullyQualifiedName~Ex040_
public class Ex040_CommandIsExecutingViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    public ReactiveCommand<RxVoid, string> RunCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _isBusy;
    public bool IsBusy => _isBusy.Value;

    /// <summary>
    /// TODO:
    ///   RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    ///   _isBusy = RunCommand.IsExecuting.ToProperty(this, x => x.IsBusy);
    /// </summary>
    public Ex040_CommandIsExecutingViewModel(Func<Task<string>> work)
    {
        _work = work;
        throw new NotImplementedException(
            "TODO: Ex040 - RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread); " +
            "wire IsBusy from RunCommand.IsExecuting via ToProperty");
    }
}
