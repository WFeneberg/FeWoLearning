namespace FeWoLearning.Architecture.Exercises.Runtime.Ex093;

/// <summary>
/// A buffer worth reusing. It carries state from whoever used it last, which is the whole
/// hazard.
/// </summary>
public sealed class RenderBuffer
{
    public List<string> Lines { get; } = [];

    public string? Title { get; set; }

    public int TimesRented { get; internal set; }
}

// Exercise 093 — ObjectPoolReuse (runtime).
// Goal:   Reuse an allocation without handing the next caller the last caller's data.
// Drills: pooling mutable objects, resetting, where the reset belongs, growth.
// Passes: reuse    - renting after a return gives back the same instance, with its rent
//                    count climbing.
//         THE ONE   - the instance comes back CLEAN. No lines, no title. A pooled object
//                    that remembers is how one customer's invoice gets another customer's
//                    address, and nothing anywhere throws.
//         growth   - renting more than the pool holds creates a new one rather than
//                    failing. An object pool is an allocation optimisation, not a
//                    capacity limit - which is exactly where it differs from exercise 092.
//         capping  - returning more objects than the pool's capacity keeps only that many;
//                    the rest are simply dropped, or the "pool" is an unbounded cache of
//                    things nobody is using.
//         THE SECOND- the reset happens on RETURN, not on rent. A dirty object sitting in
//                    the pool holds every string it captured alive until somebody happens
//                    to rent it, which is a memory leak that looks like normal usage.
//
// This is the sibling of exercise 092 and the differences are the lesson. A connection pool
// bounds a scarce external resource, so exhaustion has to be an error. An object pool
// bounds nothing - the runtime will happily allocate another buffer - so exhaustion just
// means "allocate one". Getting these backwards produces either a connection pool that
// silently opens unlimited connections, or an object pool that throws under load.
public sealed class BufferPool(int capacity)
{
    private readonly Stack<RenderBuffer> _idle = new();

    public int Idle => _idle.Count;

    public int Created { get; private set; }

    public RenderBuffer Rent() =>
        throw new NotImplementedException(
            "TODO: Ex093 - take an idle buffer or create one, counting creations and rents");

    public void Return(RenderBuffer buffer) =>
        throw new NotImplementedException(
            "TODO: Ex093 - clear the buffer, then keep it only if there is room");
}
