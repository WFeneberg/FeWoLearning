using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex054;

public enum CircuitState
{
    Closed,
    Open,
    HalfOpen,
}

public sealed class CircuitOpenException() : Exception("The circuit is open.");

// Exercise 054 — CircuitBreakerStates (reference solution).
public sealed class CircuitBreaker(IClock clock, int failureThreshold, TimeSpan breakDuration)
{
    private int _consecutiveFailures;
    private DateTimeOffset? _openedAt;

    public CircuitState State
    {
        get
        {
            if (_openedAt is not { } opened)
                return CircuitState.Closed;

            // A function of the clock, not of anybody calling. A breaker that only
            // transitions inside Execute reports Open forever if nothing tries it, which
            // makes every dashboard built on State wrong.
            return clock.UtcNow - opened >= breakDuration ? CircuitState.HalfOpen : CircuitState.Open;
        }
    }

    public T Execute<T>(Func<T> work)
    {
        if (State == CircuitState.Open)
            // work is never invoked. That is the entire point: the broken dependency gets
            // a rest, and the caller gets a fast, cheap failure instead of a slow one.
            throw new CircuitOpenException();

        var probing = State == CircuitState.HalfOpen;

        try
        {
            var result = work();

            _consecutiveFailures = 0;
            _openedAt = null;
            return result;
        }
        catch (Exception ex) when (ex is not CircuitOpenException)
        {
            if (probing)
            {
                // A failed probe re-opens immediately, restarting the break. Requiring
                // another full threshold instead means a service that is down for an hour
                // is hammered once per break duration, in bursts, all hour.
                _openedAt = clock.UtcNow;
                _consecutiveFailures = failureThreshold;
                throw;
            }

            if (++_consecutiveFailures >= failureThreshold)
                _openedAt = clock.UtcNow;

            throw;
        }
    }
}
