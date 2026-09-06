using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex068;

/// <summary>What the drain achieved. Abandoned is the number nobody wants and everybody needs.</summary>
public sealed record ShutdownReport(int Drained, int Abandoned);

// Exercise 068 — GracefulShutdown (scale).
// Goal:   Take an instance out of service without dropping the work it is already doing,
//         and without hanging for ever on the work that will never finish.
// Drills: stop-accepting-then-drain, deadlines, honest reporting.
// Passes: normal     - TryBegin admits work while running.
//         THE FIRST   - the moment shutdown starts, TryBegin REFUSES new work. Draining
//                      while still accepting is a queue that never empties.
//         draining   - shutdown waits for in-flight work and reports it as Drained.
//         THE SECOND  - past the deadline, shutdown RETURNS ANYWAY and reports what it
//                      abandoned. A drain that waits for ever is a process the
//                      orchestrator kills - and then the drain is lost too, along with
//                      any chance of saying what was in flight.
//         idempotent - shutting down twice is harmless.
//
// The order is the whole pattern, and it is the half people leave out. Draining while
// still accepting requests is not a drain: on a service under load the in-flight count
// never reaches zero, the deadline expires, and everything is abandoned - the same
// outcome as no graceful shutdown at all, after a delay.
//
// The deadline is the other half. Every orchestrator has its own patience - Kubernetes
// sends SIGTERM and then SIGKILL after terminationGracePeriodSeconds - and a process that
// waits longer than that gets killed mid-drain. Finishing early with an honest count
// beats being killed with none.
//
// Nothing here sleeps: the drain calls onWait, and the tests use it to advance the clock
// and complete requests.
public sealed class RequestHost(IClock clock)
{
    public int InFlight =>
        throw new NotImplementedException("TODO: Ex068 - how many requests are still running");

    public bool IsShuttingDown =>
        throw new NotImplementedException("TODO: Ex068 - has shutdown started");

    /// <summary>Admit a request, unless shutdown has begun.</summary>
    public bool TryBegin(string requestId) =>
        throw new NotImplementedException("TODO: Ex068 - admit while running, refuse once shutting down");

    public void Complete(string requestId) =>
        throw new NotImplementedException("TODO: Ex068 - the request has finished");

    /// <summary>
    /// Stop accepting, then wait for what is in flight. <paramref name="onWait"/> is
    /// called once per drain step - the tests advance the clock and complete requests in
    /// it. Returns once everything has drained or the deadline has passed.
    /// </summary>
    public ShutdownReport Shutdown(TimeSpan deadline, Action onWait) =>
        throw new NotImplementedException(
            "TODO: Ex068 - refuse new work FIRST, then drain until empty or until the deadline, reporting both counts");
}
