namespace FeWoLearning.Exercises.Advanced;

// Exercise 090 — Circuit breaker (advanced).
// Goal:   Wrap a flaky action with a circuit breaker: it starts Closed, trips to
//         Open after N consecutive failures (rejecting calls without invoking the
//         action), and after a reset timeout elapses reports HalfOpen so a single
//         trial call can decide whether to Close again or reopen immediately.
// Drills: state machines, resilience patterns, injecting a clock for determinism.
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
    public CircuitBreaker(int failureThreshold, TimeSpan resetTimeout, Func<DateTime> clock)
        => throw new NotImplementedException();

    public CircuitState State => throw new NotImplementedException();

    public T Execute<T>(Func<T> action) => throw new NotImplementedException();
}
