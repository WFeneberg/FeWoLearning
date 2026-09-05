// Exercise 044 - Coroutine From Task (intermediate).
// Goal:   Caliburn ships an adapter from a plain Task into the coroutine pipeline - you never
//         hand-roll an IResult around one. Task.AsResult()/Task<T>.AsResult() make a yielded task
//         something the coroutine genuinely waits for, and a faulted task surfaces wrapped in an
//         AggregateException - unlike a hand-written IResult's Error, which stays the original
//         exception, unwrapped.
// Drills: writing the sequence method yourself, yielding work.AsResult() so the coroutine truly
//         waits for it instead of racing ahead to the next log line.
// Passes: dotnet test --filter FullyQualifiedName~Ex044_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex044_CoroutineFromTask
{
    /// <summary>Runs LogAroundTask through Coroutine.ExecuteAsync.</summary>
    public static Task RunAsync(Task work, List<string> log) =>
        Coroutine.ExecuteAsync(LogAroundTask(work, log).GetEnumerator(), new CoroutineExecutionContext());

    public static IEnumerable<IResult> LogAroundTask(Task work, List<string> log)
    {
        log.Add("before");
        yield return work.AsResult();
        log.Add("after");
    }

    /// <summary>Runs CaptureValue through Coroutine.ExecuteAsync and returns the value it captured.</summary>
    public static async Task<int> RunCaptureAsync(Task<int> work)
    {
        var holder = new Ex044_ValueHolder();
        await Coroutine.ExecuteAsync(CaptureValue(work, holder).GetEnumerator(), new CoroutineExecutionContext());
        return holder.Value;
    }

    public static IEnumerable<IResult> CaptureValue(Task<int> work, Ex044_ValueHolder holder)
    {
        var adapted = work.AsResult();
        yield return adapted;
        holder.Value = adapted.Result;
    }
}

/// <summary>A simple mutable box - lets CaptureValue's iterator hand a value back to its caller
/// once execution completes, since the iterator method itself cannot return one directly.</summary>
public class Ex044_ValueHolder
{
    public int Value { get; set; }
}
