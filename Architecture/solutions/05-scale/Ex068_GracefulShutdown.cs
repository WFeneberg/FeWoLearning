using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex068;

/// <summary>What the drain achieved. Abandoned is the number nobody wants and everybody needs.</summary>
public sealed record ShutdownReport(int Drained, int Abandoned);

// Exercise 068 — GracefulShutdown (reference solution).
public sealed class RequestHost(IClock clock)
{
    private readonly HashSet<string> _inFlight = new(StringComparer.Ordinal);
    private int _drained;

    public int InFlight => _inFlight.Count;

    public bool IsShuttingDown { get; private set; }

    public bool TryBegin(string requestId)
    {
        if (IsShuttingDown)
            return false;

        return _inFlight.Add(requestId);
    }

    public void Complete(string requestId)
    {
        if (_inFlight.Remove(requestId))
            _drained++;
    }

    public ShutdownReport Shutdown(TimeSpan deadline, Action onWait)
    {
        if (IsShuttingDown)
            return new ShutdownReport(_drained, _inFlight.Count);

        // FIRST. Draining while still accepting is not a drain: under load the in-flight
        // count never reaches zero, the deadline expires, and everything is abandoned -
        // the same outcome as no graceful shutdown at all, after a delay.
        IsShuttingDown = true;
        _drained = 0;

        var expiresAt = clock.UtcNow + deadline;

        while (_inFlight.Count > 0 && clock.UtcNow < expiresAt)
            onWait();

        // Returns either way. A drain that waits for ever is a process the orchestrator
        // kills, and then the drain is lost too - along with any chance of saying what
        // was still in flight.
        return new ShutdownReport(_drained, _inFlight.Count);
    }
}
