using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Domain.Ex088;

public enum OperationState
{
    Running,
    Succeeded,
    Failed,
}

public sealed record OperationStatus(
    string Id,
    OperationState State,
    int PercentComplete,
    string? ResultLocation,
    string? Error,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt);

/// <summary>What a caller gets back: a status code, a Location to poll, and a body.</summary>
public sealed record Httpish(int StatusCode, string? Location, OperationStatus? Body);

// Exercise 088 — LongRunningOperation (reference solution).
public sealed class OperationStore(IClock clock)
{
    private readonly Dictionary<string, OperationStatus> _operations = [];

    public Httpish Start(string id)
    {
        var status = new OperationStatus(id, OperationState.Running, 0, null, null, clock.UtcNow, null);
        _operations[id] = status;

        // 202, not 200, and no result in the body - the work has not been done. Handing
        // back an id turns the idle timeout, the client timeout, the retry-starts-it-twice
        // problem and the mid-flight deploy into one boring resource somebody can poll.
        return new Httpish(202, $"/operations/{id}", status);
    }

    public void Report(string id, int percentComplete)
    {
        if (_operations.TryGetValue(id, out var status) && status.State == OperationState.Running)
            _operations[id] = status with { PercentComplete = percentComplete };
    }

    public void Succeed(string id, string resultLocation)
    {
        if (!_operations.TryGetValue(id, out var status))
            return;

        // The result is a SEPARATE resource. Returning the body here would make it
        // reachable exactly once, from exactly this poll - not cacheable, not
        // re-fetchable, not linkable.
        _operations[id] = status with
        {
            State = OperationState.Succeeded,
            PercentComplete = 100,
            ResultLocation = resultLocation,
            CompletedAt = clock.UtcNow,
        };
    }

    public void Fail(string id, string error)
    {
        if (!_operations.TryGetValue(id, out var status))
            return;

        _operations[id] = status with
        {
            State = OperationState.Failed,
            Error = error,
            CompletedAt = clock.UtcNow,
        };
    }

    public Httpish Poll(string id) =>
        _operations.TryGetValue(id, out var status)
            // 200 even when the work FAILED. The poll asked "how is it going" and the
            // answer - "badly" - arrived successfully. A 500 fires the client's transport
            // error handling for a transport problem it does not have, and most clients
            // then retry the poll, which changes nothing.
            ? new Httpish(200, null, status)
            : new Httpish(404, null, null);
}
