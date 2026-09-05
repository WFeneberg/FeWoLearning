using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 041 - CommandThrownExceptions (intermediate).
/// Goal:   Wrap a given async unit of work in a ReactiveCommand, and make sure a
///         failure is OBSERVED (surfaced on the view model) instead of becoming
///         an unobserved exception that can crash the app.
/// Drills: ThrownExceptions, no unobserved crash.
/// Passes: dotnet test --filter FullyQualifiedName~Ex041_
public class Ex041_CommandThrownExceptionsViewModel : ReactiveObject
{
    private readonly Func<Task<string>> _work;

    private string? _lastError;
    public string? LastError { get => _lastError; private set => this.RaiseAndSetIfChanged(ref _lastError, value); }

    public ReactiveCommand<RxVoid, string> FetchCommand { get; }

    public Ex041_CommandThrownExceptionsViewModel(Func<Task<string>> work)
    {
        _work = work;
        FetchCommand = ReactiveCommand.CreateFromTask(_work, Sequencer.CurrentThread);
        FetchCommand.ThrownExceptions.Subscribe(ex => LastError = ex.Message);
    }
}
