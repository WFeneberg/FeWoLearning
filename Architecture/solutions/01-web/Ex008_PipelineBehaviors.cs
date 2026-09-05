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

// Exercise 008 — PipelineBehaviors (reference solution).
public static class Ex008_PipelineBehaviors
{
    public static Func<string, Task<string>> Compose(
        IReadOnlyList<IPipelineBehavior> behaviors,
        Func<string, Task<string>> handler)
    {
        var pipeline = handler;

        // Back to front, so that behaviors[0] is applied last and therefore ends up
        // outermost.
        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];

            // `next` MUST be a fresh local capturing the pipeline as it stands right
            // now. Capturing `pipeline` itself would give every layer a reference to
            // the variable, which by the end of the loop points at the outermost
            // delegate - and the first invocation calls itself until the stack goes.
            var next = pipeline;

            pipeline = request => behavior.Handle(request, next);
        }

        return pipeline;
    }
}
