using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex064;

// Exercise 064 — CostAwareBatching (scale).
// Goal:   Trade latency for throughput on purpose, with a bound on how much latency any
//         single item can be made to pay.
// Drills: size-triggered vs age-triggered flushing, latency bounds, partial batches.
// Passes: size      - the maxSize-th Add flushes immediately, with all of them, and the
//                     buffer is empty afterwards.
//         patience  - fewer than maxSize items do not flush on their own.
//         age       - Tick flushes once the OLDEST buffered item has been waiting maxAge,
//                     however few there are.
//         THE ONE    - the age is measured from the oldest ITEM, not from the last flush.
//                     An item added just now must not be flushed because the previous
//                     batch went out a long time ago.
//         empty     - Tick on an empty buffer flushes nothing. A flush handler must never
//                     be handed an empty batch.
//
// Batching is the one optimisation that makes the average case better and the worst case
// worse, and the age trigger is what bounds the worst case. Without it, the last few
// items of the day sit in the buffer until tomorrow's traffic pushes them out - which
// looks fine in every load test, because a load test never stops sending.
//
// Measuring the age from the last FLUSH instead of from the oldest item is the natural
// mistake and it is subtly wrong in both directions: it flushes a fresh item early after
// an idle period, and - worse - it resets on every flush, so a steady trickle can leave
// an item waiting almost 2 x maxAge. The guarantee people believe they are buying is
// "no item waits longer than maxAge", and only the oldest-item reading delivers it.
public sealed class Batcher<T>(IClock clock, int maxSize, TimeSpan maxAge, Action<IReadOnlyList<T>> flush)
{
    public int Pending =>
        throw new NotImplementedException("TODO: Ex064 - how many items are waiting");

    /// <summary>Buffer an item, flushing immediately if the batch is now full.</summary>
    public void Add(T item) =>
        throw new NotImplementedException(
            "TODO: Ex064 - buffer the item, remembering when the batch started, and flush when it reaches maxSize");

    /// <summary>Flush if the oldest buffered item has been waiting at least maxAge.</summary>
    public void Tick() =>
        throw new NotImplementedException(
            "TODO: Ex064 - flush only when there is something buffered AND its oldest item has waited maxAge");

    /// <summary>Flush whatever is there. Does nothing when there is nothing.</summary>
    public void Flush() =>
        throw new NotImplementedException("TODO: Ex064 - hand the buffered items to the flush handler and clear, unless empty");
}
