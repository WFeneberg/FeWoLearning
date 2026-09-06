namespace FeWoLearning.Architecture.Exercises.Scale.Ex062;

public sealed class LoadSheddingException() : Exception("The service is at capacity; try again later.");

// Exercise 062 — ConcurrencyLimiter (scale).
// Goal:   Decide what to do with work that arrives faster than it can be done: run it,
//         queue it briefly, or refuse it outright.
// Drills: admission control, bounded queueing, load shedding.
// Passes: running   - up to `concurrency` callers run at once.
//         queued    - the next `queueDepth` callers WAIT. They are not running and not
//                     rejected; Queued reports them.
//         THE ONE    - beyond that, a caller is refused IMMEDIATELY with
//                     LoadSheddingException, and its work is never invoked.
//         draining  - when a running caller finishes, a queued one starts.
//         release   - a caller that throws still frees its slot.
//
// Where exercise 061 partitions capacity between dependencies, this one decides what
// happens at the edge of whatever capacity you have. The queue is the interesting part,
// and its DEPTH is the whole design: with none, a momentary burst is refused although
// the service could have absorbed it; with an unbounded one, nothing is ever refused
// and the queue grows until every request in it has already timed out on the client
// side - so the service spends all its capacity computing answers nobody is waiting for
// any more. That state is stable, self-sustaining, and looks from outside like a total
// outage.
//
// Refusing fast is a feature. A client that is told "no" in a millisecond can fail over,
// retry elsewhere, or show a message; one left in a queue for thirty seconds cannot do
// any of those, and neither can the service.
public sealed class AdmissionController(int concurrency, int queueDepth)
{
    public int Running =>
        throw new NotImplementedException("TODO: Ex062 - how many callers are executing right now");

    public int Queued =>
        throw new NotImplementedException("TODO: Ex062 - how many callers are waiting for a slot");

    /// <summary>
    /// Run <paramref name="work"/> now, or wait for a slot if there is queue room, or
    /// refuse immediately.
    /// </summary>
    public Task<T> ExecuteAsync<T>(Func<Task<T>> work) =>
        throw new NotImplementedException(
            "TODO: Ex062 - admit up to concurrency, queue up to queueDepth more, and refuse anything beyond that without calling work");
}
