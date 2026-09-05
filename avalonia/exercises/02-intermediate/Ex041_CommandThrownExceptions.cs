using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 041 - CommandThrownExceptions (intermediate).
/// Goal:   Wrap a given async unit of work in a ReactiveCommand, and make sure a
///         failure is OBSERVED (surfaced on the view model) instead of becoming
///         an unobserved exception that can crash the app.
/// Drills: ThrownExceptions, no unobserved crash.
///
/// Measured on this machine against ReactiveUI 24.1.0, with Sequencer.CurrentThread:
///   var boom = ReactiveCommand.CreateFromTask&lt;RxVoid, string&gt;(
///       _ => Task.FromException&lt;string&gt;(new InvalidOperationException("boom")),
///       Sequencer.CurrentThread);
///   using var esub = boom.ThrownExceptions.Subscribe(e => errors.Add(e.Message));
/// Both channels see the failure: errors received "boom", AND awaiting
/// boom.Execute().ToTask() also threw InvalidOperationException("boom"). A
/// ReactiveCommand whose ThrownExceptions has no subscriber risks tearing down
/// the whole app on the next failure - subscribing is what turns that into a
/// handled, observable event instead.
/// Passes: dotnet test --filter FullyQualifiedName~Ex041_
public class Ex041_CommandThrownExceptionsViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string? _lastError;
    public string? LastError { get => _lastError; private set => this.RaiseAndSetIfChanged(ref _lastError, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    /// <summary>
    /// TODO:
    ///   FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
    ///   FetchCommand.ThrownExceptions.Subscribe(ex => LastError = ex.Message);
    /// Do NOT swallow the failure yourself (no try/catch around _work()) - the whole
    /// point is that ReactiveCommand's own ThrownExceptions channel is what observes
    /// the failure. A wrapper that catches and returns a fallback value instead would
    /// make the command "succeed", which is exactly the bug this exercise is about.
    /// </summary>
    public Ex041_CommandThrownExceptionsViewModel(Func<Task<string>> work)
    {
        _work = work;
        throw new NotImplementedException(
            "TODO: Ex041 - FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread), " +
            "and subscribe FetchCommand.ThrownExceptions to store ex.Message in LastError");
    }
}
