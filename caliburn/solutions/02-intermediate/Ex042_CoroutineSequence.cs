// Exercise 042 - Coroutine Sequence (intermediate).
// Goal:   A `yield return` chain runs strictly in order: Coroutine.ExecuteAsync drives the
//         IEnumerator<IResult> one MoveNext() at a time, and only asks for the next step once the
//         current one's Completed has fired. Nothing about the chain runs concurrently, and
//         nothing runs at all until something actually enumerates it.
// Drills: writing an iterator method (`yield return`) that chains three already-working IResult
//         steps together, in the order this exercise is named after - Coroutine.ExecuteAsync
//         wants an IEnumerator<IResult>, so the sequence is exposed as IEnumerable<IResult> and
//         handed off via .GetEnumerator().
// Passes: dotnet test --filter FullyQualifiedName~Ex042_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex042_CoroutineSequence
{
    /// <summary>Runs first, then second, then third, through Coroutine.ExecuteAsync.</summary>
    public static Task RunInOrderAsync(IResult first, IResult second, IResult third) =>
        Coroutine.ExecuteAsync(Steps(first, second, third).GetEnumerator(), new CoroutineExecutionContext());

    public static IEnumerable<IResult> Steps(IResult first, IResult second, IResult third)
    {
        yield return first;
        yield return second;
        yield return third;
    }
}

/// <summary>An already-working coroutine step: appends its own Name to a shared log, then
/// completes immediately.</summary>
public class Ex042_LoggingStep : IResult
{
    private readonly List<string> _log;

    public Ex042_LoggingStep(List<string> log, string name)
    {
        _log = log;
        Name = name;
    }

    public string Name { get; }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    public void Execute(CoroutineExecutionContext context)
    {
        _log.Add(Name);
        Completed?.Invoke(this, new ResultCompletionEventArgs());
    }
}

/// <summary>An already-working coroutine step that finishes asynchronously: logs a "start" entry
/// immediately, then a "done" entry after a delay, before raising Completed - used to prove the
/// coroutine genuinely waits rather than racing ahead to the next step.</summary>
public class Ex042_DelayedStep : IResult
{
    private readonly List<string> _log;
    private readonly string _startEntry;
    private readonly string _doneEntry;
    private readonly TimeSpan _delay;

    public Ex042_DelayedStep(List<string> log, string startEntry, string doneEntry, TimeSpan delay)
    {
        _log = log;
        _startEntry = startEntry;
        _doneEntry = doneEntry;
        _delay = delay;
    }

    public event EventHandler<ResultCompletionEventArgs>? Completed;

    public void Execute(CoroutineExecutionContext context)
    {
        _log.Add(_startEntry);
        _ = FinishAsync();

        async Task FinishAsync()
        {
            await Task.Delay(_delay);
            _log.Add(_doneEntry);
            Completed?.Invoke(this, new ResultCompletionEventArgs());
        }
    }
}
