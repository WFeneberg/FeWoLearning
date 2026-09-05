namespace FeWoLearning.Architecture.Exercises.Web.Ex008;

/// <summary>Shared log, so the order things ran in is observable at all.</summary>
public sealed class Recorder
{
    public List<string> Entries { get; } = [];
}

/// <summary>
/// A behaviour wraps the rest of the pipeline. It gets the request and a delegate that
/// runs everything further in - which means it can act before, act after, change the
/// result, or decline to call it at all.
/// </summary>
public interface IPipelineBehavior
{
    Task<string> Handle(string request, Func<string, Task<string>> next);
}

public sealed class FirstBehavior(Recorder recorder) : IPipelineBehavior
{
    public async Task<string> Handle(string request, Func<string, Task<string>> next)
    {
        recorder.Entries.Add("first:in");
        var result = await next(request);
        recorder.Entries.Add("first:out");
        return "[" + result + "]";
    }
}

public sealed class SecondBehavior(Recorder recorder) : IPipelineBehavior
{
    public async Task<string> Handle(string request, Func<string, Task<string>> next)
    {
        recorder.Entries.Add("second:in");
        var result = await next(request);
        recorder.Entries.Add("second:out");
        return result;
    }
}

/// <summary>Declines to call the rest of the pipeline for one particular request.</summary>
public sealed class StopBehavior(Recorder recorder) : IPipelineBehavior
{
    public Task<string> Handle(string request, Func<string, Task<string>> next)
    {
        if (request == "stop")
        {
            recorder.Entries.Add("stop:short-circuit");
            return Task.FromResult("stopped");
        }

        return next(request);
    }
}

// Exercise 008 — PipelineBehaviors (web).
// Goal:   Fold a list of behaviours around a handler so that the FIRST behaviour in the
//         list is the OUTERMOST one.
// Drills: decorator chain, ordering, composition, closure capture.
// Passes: [First, Second] around a handler that records "handler" - the log reads
//                 ["first:in", "second:in", "handler", "second:out", "first:out"] and
//                 the result is "[handled:x]", because the outermost behaviour is the
//                 last to touch the value on the way out.
//         []       - the handler runs alone and its result is returned unchanged.
//         [First, Stop, Second] with request "stop" - the handler never runs, no
//                 "second:in" is recorded, and "first:out" still is.
//         the composed delegate is reusable across invocations.
//
// The trap: folding from the back of the list, each step has to capture the CURRENT
// accumulated delegate in its own local. Capturing the loop variable instead gives you
// a delegate that calls itself, and the first invocation stack-overflows.
public static class Ex008_PipelineBehaviors
{
    /// <summary>
    /// Wrap <paramref name="handler"/> in <paramref name="behaviors"/> and return the
    /// composed delegate. behaviors[0] is outermost.
    /// </summary>
    public static Func<string, Task<string>> Compose(
        IReadOnlyList<IPipelineBehavior> behaviors,
        Func<string, Task<string>> handler) =>
        throw new NotImplementedException(
            "TODO: Ex008 - fold the behaviours around the handler so behaviors[0] ends up outermost");
}
