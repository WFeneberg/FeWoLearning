using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 042 - CommandCancellation (intermediate).
/// Goal:   Wrap a given cancellable async unit of work in a ReactiveCommand, so
///         disposing an in-flight execution's subscription genuinely cancels it.
/// Drills: CancellationToken in CreateFromTask.
///
/// Measured on this machine against ReactiveUI 24.1.0: disposing the subscription
/// returned by command.Execute() cancels the CancellationToken that ReactiveCommand
/// hands to a Func&lt;CancellationToken, Task&lt;T&gt;&gt; factory -
///   var sub = slow.Execute().Subscribe(_ => { }, _ => { });
///   await started.Task;
///   sub.Dispose();   // this cancels the token _work receives
/// This is entirely CreateFromTask's own machinery - the ONE thing this exercise
/// asks you to get right is USING the overload that takes the token at all and
/// forwarding it straight through. A wrapper that discards the real token (e.g.
/// invoking _work(CancellationToken.None) instead of _work(ct)) still compiles and
/// still "looks" cancellable, but the work never observes the cancellation - the
/// test drives this exact case.
/// Passes: dotnet test --filter FullyQualifiedName~Ex042_
public class Ex042_CommandCancellationViewModel : ReactiveObject
{
    private readonly Func<CancellationToken, Task<string>> _work;

    public ReactiveCommand<RxVoid, string> RunCommand { get; }

    /// <summary>
    /// TODO:
    ///   RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    /// Forward the token CreateFromTask gives you straight into _work - do not
    /// substitute CancellationToken.None and do not swallow it.
    /// </summary>
    public Ex042_CommandCancellationViewModel(Func<CancellationToken, Task<string>> work)
    {
        _work = work;
        throw new NotImplementedException(
            "TODO: Ex042 - RunCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread); " +
            "forward the real CancellationToken through, do not discard it");
    }
}
