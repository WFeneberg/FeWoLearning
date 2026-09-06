using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex054;

public enum CircuitState
{
    Closed,
    Open,
    HalfOpen,
}

public sealed class CircuitOpenException() : Exception("The circuit is open.");

// Exercise 054 — CircuitBreakerStates (cross-cutting).
// Goal:   Stop calling something that is clearly broken, and find out when it is better
//         WITHOUT going back to hammering it.
// Drills: closed/open/half-open transitions, the probe, virtual time.
// Passes: closed     - successes keep it closed; the failure count resets on a success.
//         opening    - failureThreshold consecutive failures open it.
//         open       - Execute throws CircuitOpenException and the work is NOT CALLED AT
//                      ALL. That is the entire point: the broken dependency gets a rest.
//         half-open  - once breakDuration has passed, State reports HalfOpen and the next
//                      call is let through as a probe.
//         THE ONE     - a FAILING probe re-opens the circuit immediately: the call after
//                      it is rejected again, without waiting for a second threshold.
//         recovery   - a succeeding probe closes the circuit and clears the count.
//
// The probe is what people leave out, and without it there are two ways to be wrong. A
// breaker that simply closes after breakDuration sends the whole backlog at a service
// that may still be down - which is the stampede the breaker was installed to prevent,
// merely delayed. And one that never re-opens after a failed probe needs another full
// threshold of failures to trip again, so a service that is down for an hour is hammered
// once per break duration, in bursts, all hour.
//
// State is a function of the clock: a breaker that opened long enough ago is half-open
// whether or not anybody has called it since.
public sealed class CircuitBreaker(IClock clock, int failureThreshold, TimeSpan breakDuration)
{
    public CircuitState State =>
        throw new NotImplementedException(
            "TODO: Ex054 - Closed, Open, or HalfOpen once breakDuration has elapsed since it opened");

    /// <summary>
    /// Run <paramref name="work"/> unless the circuit is open. A failure counts towards
    /// the threshold; a success resets it.
    /// </summary>
    public T Execute<T>(Func<T> work) =>
        throw new NotImplementedException(
            "TODO: Ex054 - reject without calling work while open, let one probe through when half-open, and re-open immediately if it fails");
}
