namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex039;

/// <summary>
/// A read-modify-write with a deliberate suspension point in the middle - which is what
/// every real one has, whether the author noticed or not: a network round trip to the
/// database, a call to a pricing service, a log write.
/// </summary>
public sealed class Balances
{
    private readonly Dictionary<string, int> _values = [];

    public int Read(string account) => _values.GetValueOrDefault(account);

    public void Write(string account, int value) => _values[account] = value;
}

// Exercise 039 — PessimisticLocking (services-data).
// Goal:   Serialise writers on the SAME key while letting writers on different keys run
//         at the same time.
// Drills: per-key mutual exclusion, lost updates, lock scope, release on failure.
// Passes: sequential  - increments accumulate.
//         THE ONE      - N concurrent increments of ONE account all land: the final
//                        balance is N. Without the lock the read-modify-write loses
//                        updates, and the yield in the middle makes that deterministic
//                        rather than a matter of luck.
//         per key      - two DIFFERENT accounts are incremented CONCURRENTLY. A single
//                        global lock passes the fact above and serialises the whole
//                        system.
//         failure      - an increment that throws still releases the key, so the next
//                        caller is not stuck behind a lock nobody holds.
//
// Where 038 lets both writers run and tells the loser afterwards, this one stops the
// second writer getting in at all. The trade is the same one every time: optimistic
// costs nothing until there IS contention, pessimistic costs waiting always but never
// asks anybody to redo their work.
public sealed class KeyedLockingBalances(Balances balances)
{
    /// <summary>
    /// Add <paramref name="amount"/> to <paramref name="account"/>, holding the account's
    /// lock across the whole read-modify-write.
    /// <paramref name="betweenReadAndWrite"/> runs while the lock is held; the tests use
    /// it to force the interleaving that loses an update.
    /// </summary>
    public Task IncrementAsync(string account, int amount, Func<Task>? betweenReadAndWrite = null) =>
        throw new NotImplementedException(
            "TODO: Ex039 - take a lock for THIS account, read, run the callback, write, and release even if it throws");
}
