// Exercise 065 - MVUX State Updates (intermediate).
// Goal:   Own mutable state the MVUX way, and know which read tells the truth.
// Drills: State.Value with an owner, Update against Set, the stream of values a state
//         produces, and the read that lags inside a live SourceContext.
// Passes: dotnet test --filter FullyQualifiedName~Ex065_
//
// A State is a Feed you can write to. The owner matters: MVUX attaches the state to it, so
// two calls to State.Value with the same owner and key hand back the same state, and the
// state dies with its owner rather than with the variable that referenced it.
//
// The trap is the read. `await state.Value(ct)` outside any subscription materialises the
// current value and is correct. Inside a live SourceContext - which is what a bound view
// establishes - it answers from the subscription's last *propagated* message, and
// propagation is asynchronous: read it straight after an Update and it is one behind. This
// is why MVUX code reads state through a feed pipeline or a binding, and why a test that
// asserts on a value read inside a context is testing the timing rather than the logic.

using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A counter that owns its state. Being the owner is the point: the state's lifetime is
/// this object's.
/// </summary>
public sealed class Ex065_Counter
{
    /// <summary>
    /// The count, seeded with <paramref name="seed"/> the first time it is asked for.
    /// </summary>
    public Ex065_Counter(int seed) => Seed = seed;

    /// <summary>The value the state starts from.</summary>
    public int Seed { get; }

    /// <summary>
    /// The state itself. The same instance every time - MVUX keys it by owner, so asking
    /// twice must not produce two states.
    /// </summary>
    public IState<int> Count =>
        // TODO: State.Value takes the owner and a factory for the seed. `this` is the
        // owner; a new state per call would make every reader watch a different one.
        throw new NotImplementedException("TODO: Ex065 - expose the counter's state");

    /// <summary>Raises the count by one, from whatever it currently is.</summary>
    public async Task IncrementAsync(CancellationToken ct) =>
        // TODO: Update takes the current value and returns the next. Reading the value and
        // then Setting it would lose a concurrent update in between.
        throw new NotImplementedException("TODO: Ex065 - increment through the state");

    /// <summary>Puts the count back to the seed, whatever it was.</summary>
    public async Task ResetAsync(CancellationToken ct) =>
        // TODO: Set replaces the value outright - the right call when the new value does
        // not depend on the old one.
        throw new NotImplementedException("TODO: Ex065 - reset through the state");
}

public static class Ex065_MvuxStateUpdates
{
    /// <summary>
    /// The state's current value, read so that the answer is current.
    /// </summary>
    public static async ValueTask<int> CurrentAsync(IState<int> state, CancellationToken ct) =>
        // TODO: read the value. Note what this method must *not* do: establish a
        // SourceContext around the read.
        throw new NotImplementedException("TODO: Ex065 - read the state's current value");
}
