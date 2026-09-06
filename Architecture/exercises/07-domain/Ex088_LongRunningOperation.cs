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

// Exercise 088 — LongRunningOperation (domain).
// Goal:   Answer a request whose work takes minutes, without holding the connection open
//         for minutes.
// Drills: 202 Accepted, a status resource, polling, where the result lives.
// Passes: start     - returns 202 with a Location pointing at the status resource, and
//                     does NOT return the result. The work has not been done yet.
//         polling   - the status resource returns 200 with the state and progress while
//                     it runs.
//         THE ONE    - once it succeeds, the status carries a ResultLocation. The result is
//                     a SEPARATE resource, so it can be cached, re-fetched and linked to
//                     without re-running anything.
//         failure   - a failed operation is still 200 on the status resource, with State
//                     Failed and the reason. The polling call succeeded; the work did not,
//                     and conflating those two is how a client retries the wrong thing.
//         unknown   - polling an id nobody started is 404.
//
// The shape exists because an HTTP request is not a good place to keep a fifteen-minute
// job: the load balancer has an idle timeout, the client has one too, a retry starts the
// work a second time, and a deploy in the middle loses it. Handing back an id turns all
// four problems into one boring one - a resource somebody can poll.
//
// The status being 200 for a FAILED operation is the clause that gets argued about and it
// is worth insisting on. The poll asked "how is it going"; the answer is "badly", and that
// answer arrived successfully. Returning 500 makes the client's error handling fire for a
// transport problem it does not have, and most clients will retry the POLL, which changes
// nothing at all.
public sealed class OperationStore(IClock clock)
{
    private readonly Dictionary<string, OperationStatus> _operations = [];

    /// <summary>Accept the work and hand back somewhere to watch it.</summary>
    public Httpish Start(string id) =>
        throw new NotImplementedException(
            "TODO: Ex088 - record it as Running at 0%, and return 202 with a Location of /operations/{the id}");

    public void Report(string id, int percentComplete) =>
        throw new NotImplementedException("TODO: Ex088 - update the progress of a running operation");

    public void Succeed(string id, string resultLocation) =>
        throw new NotImplementedException("TODO: Ex088 - mark it Succeeded at 100%, with where the result lives and when it finished");

    public void Fail(string id, string error) =>
        throw new NotImplementedException("TODO: Ex088 - mark it Failed with the reason and when it finished");

    /// <summary>The status resource a client polls.</summary>
    public Httpish Poll(string id) =>
        throw new NotImplementedException(
            "TODO: Ex088 - 404 for an unknown id, otherwise 200 with the status - including when the work failed");
}
