namespace FeWoLearning.Exercises.Advanced;

// Exercise 090 — Circuit breaker (reference solution).
// A lock-protected state machine driven by an injected clock so tests can advance
// "time" deterministically instead of sleeping.
public enum CircuitState
{
    Closed,
    Open,
    HalfOpen,
}

public sealed class CircuitBreakerOpenException : Exception
{
    public CircuitBreakerOpenException(string message) : base(message) { }
}

public sealed class CircuitBreaker
{
    private readonly object _gate = new();
    private readonly int _failureThreshold;
    private readonly TimeSpan _resetTimeout;
    private readonly Func<DateTime> _clock;

    private CircuitState _state = CircuitState.Closed;
    private int _consecutiveFailures;
    private DateTime _openedAt;

    public CircuitBreaker(int failureThreshold, TimeSpan resetTimeout, Func<DateTime> clock)
    {
        if (failureThreshold <= 0)
            throw new ArgumentOutOfRangeException(nameof(failureThreshold), failureThreshold, "Must be positive.");
        if (resetTimeout < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(resetTimeout), resetTimeout, "Must be non-negative.");

        _failureThreshold = failureThreshold;
        _resetTimeout = resetTimeout;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    // Read-only projection: if we're Open and the timeout has elapsed, we report
    // HalfOpen without mutating internal state — the actual transition only
    // happens once a trial call is attempted through Execute.
    public CircuitState State
    {
        get
        {
            lock (_gate)
            {
                if (_state == CircuitState.Open && _clock() - _openedAt >= _resetTimeout)
                    return CircuitState.HalfOpen;
                return _state;
            }
        }
    }

    public T Execute<T>(Func<T> action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        lock (_gate)
        {
            if (_state == CircuitState.Open)
            {
                if (_clock() - _openedAt >= _resetTimeout)
                    _state = CircuitState.HalfOpen;
                else
                    throw new CircuitBreakerOpenException("Circuit is open; call rejected.");
            }
        }

        try
        {
            var result = action();
            lock (_gate)
            {
                _consecutiveFailures = 0;
                _state = CircuitState.Closed;
            }
            return result;
        }
        catch
        {
            lock (_gate)
            {
                _consecutiveFailures++;
                // A failed trial call while half-open reopens immediately,
                // regardless of the configured threshold.
                if (_state == CircuitState.HalfOpen || _consecutiveFailures >= _failureThreshold)
                {
                    _state = CircuitState.Open;
                    _openedAt = _clock();
                }
            }
            throw;
        }
    }
}
