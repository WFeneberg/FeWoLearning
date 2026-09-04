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
    // Keyed by owner, so this hands back the same state every time. MVUX attaches it to
    // `this`, which is also what ties its lifetime to this object rather than to a field.
    public IState<int> Count => State.Value(this, () => Seed);

    /// <summary>Raises the count by one, from whatever it currently is.</summary>
    public async Task IncrementAsync(CancellationToken ct) =>
        // Update, not read-then-Set: the framework applies the function to whatever the
        // current value is, so a concurrent update in between is not lost.
        await Count.Update(current => current + 1, ct);

    /// <summary>Puts the count back to the seed, whatever it was.</summary>
    public async Task ResetAsync(CancellationToken ct) =>
        // Set, because the new value does not depend on the old one.
        await Count.Set(Seed, ct);
}

public static class Ex065_MvuxStateUpdates
{
    /// <summary>
    /// The state's current value, read so that the answer is current.
    /// </summary>
    public static async ValueTask<int> CurrentAsync(IState<int> state, CancellationToken ct) =>
        // A plain read, with no context opened around it. Inside a live SourceContext the
        // same call answers from the subscription's last propagated message and is one
        // behind - which is the trap this exercise exists for.
        await state.Value(ct);
}
