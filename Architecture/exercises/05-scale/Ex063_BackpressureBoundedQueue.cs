namespace FeWoLearning.Architecture.Exercises.Scale.Ex063;

/// <summary>What a full buffer does to the writer.</summary>
public enum FullPolicy
{
    /// <summary>Make the producer wait. Backpressure reaches all the way up.</summary>
    Wait,

    /// <summary>Keep the newest, discard the oldest. For state, where only "now" matters.</summary>
    DropOldest,

    /// <summary>Keep what is already queued, discard the arrival. For events, where order matters.</summary>
    DropNewest,
}

// Exercise 063 — BackpressureBoundedQueue (scale).
// Goal:   Make the buffer between a fast producer and a slow consumer FINITE, and choose
//         deliberately what happens when it fills.
// Drills: bounded buffers, backpressure, drop policies, the unbounded-queue failure.
// Passes: capacity   - writes up to capacity are accepted; Count reports them.
//         DropNewest - the write past capacity is refused; the buffer still holds the
//                      FIRST capacity items, in order.
//         DropOldest - the write past capacity is accepted, and the OLDEST item is gone.
//                      The pair is what makes the choice observable at all.
//         counting   - Dropped counts what was discarded, under both policies.
//         Wait       - the producer BLOCKS until a read frees a slot, and completes then.
//         reading    - items come out in the order they went in.
//
// An unbounded queue is not the absence of a policy, it is a policy: "grow until the
// process runs out of memory, and lose everything at once". The three policies here are
// the three honest answers, and they are not interchangeable. Wait pushes the pressure
// back to whoever is producing, which is right when that producer can slow down and
// wrong when it is a network socket. DropOldest is right for state - the current
// temperature, the latest position - where a stale value has no worth. DropNewest is
// right for events, where the first ones tell you what started, and losing the middle
// of a sequence is worse than losing its tail.
//
// What all three have in common is that the loss is VISIBLE: Dropped is a number
// somebody can alert on. Silent loss is the failure mode that outlives everybody who
// understood the system.
public sealed class BoundedBuffer<T>(int capacity, FullPolicy policy)
{
    public int Count =>
        throw new NotImplementedException("TODO: Ex063 - how many items are buffered");

    public int Dropped =>
        throw new NotImplementedException("TODO: Ex063 - how many items were discarded");

    /// <summary>
    /// Write under a drop policy. Returns whether the item was accepted. Not for
    /// <see cref="FullPolicy.Wait"/>.
    /// </summary>
    public bool TryWrite(T item) =>
        throw new NotImplementedException(
            "TODO: Ex063 - accept while there is room; when full, drop the oldest or refuse the newest, counting either way");

    /// <summary>Write under <see cref="FullPolicy.Wait"/>: completes once there is room.</summary>
    public Task WriteAsync(T item) =>
        throw new NotImplementedException(
            "TODO: Ex063 - complete immediately when there is room, otherwise complete once a read frees a slot");

    public bool TryRead(out T item) =>
        throw new NotImplementedException("TODO: Ex063 - take the oldest item, releasing a slot for any waiting writer");
}
