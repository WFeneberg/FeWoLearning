namespace FeWoLearning.Architecture.Exercises.Web.Ex014;

/// <summary>One upstream service the BFF fans out to.</summary>
public interface IUpstream
{
    string Name { get; }

    Task<string> FetchAsync(CancellationToken cancellationToken);
}

/// <summary>
/// What came back, and what did not. Errors are DATA here, not an exception: the whole
/// point of a BFF is that a page with three panels renders two of them when the third
/// service is down.
/// </summary>
public sealed record AggregateResult(
    IReadOnlyDictionary<string, string> Data,
    IReadOnlyDictionary<string, string> Errors);

// Exercise 014 — BackendForFrontend (reference solution).
public static class Ex014_BackendForFrontend
{
    public static async Task<AggregateResult> AggregateAsync(
        IReadOnlyList<IUpstream> upstreams,
        CancellationToken cancellationToken = default)
    {
        // Start everything first, await afterwards. Awaiting inside the loop is the
        // one-character difference between a fan-out and a queue.
        var inFlight = upstreams
            .Select(u => (Upstream: u, Task: Attempt(u, cancellationToken)))
            .ToList();

        await Task.WhenAll(inFlight.Select(x => x.Task));

        var data = new Dictionary<string, string>();
        var errors = new Dictionary<string, string>();

        foreach (var (upstream, task) in inFlight)
        {
            var (value, error) = task.Result;
            if (error is null)
                data[upstream.Name] = value!;
            else
                errors[upstream.Name] = error;
        }

        return new AggregateResult(data, errors);
    }

    /// <summary>
    /// Each upstream's failure is caught HERE, per task, rather than around the
    /// WhenAll. Task.WhenAll surfaces only the first exception, so catching around it
    /// would lose which upstream failed and hide any second failure entirely.
    /// </summary>
    private static async Task<(string? Value, string? Error)> Attempt(
        IUpstream upstream, CancellationToken cancellationToken)
    {
        try
        {
            return (await upstream.FetchAsync(cancellationToken), null);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }
}
