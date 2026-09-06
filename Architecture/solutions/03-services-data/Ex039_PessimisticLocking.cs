using System.Collections.Concurrent;

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

// Exercise 039 — PessimisticLocking (reference solution).
public sealed class KeyedLockingBalances(Balances balances)
{
    // One semaphore PER ACCOUNT. A single lock would also make every increment correct -
    // and would serialise every unrelated account in the system behind whichever one is
    // slowest, which is the difference between a row lock and a table lock.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task IncrementAsync(string account, int amount, Func<Task>? betweenReadAndWrite = null)
    {
        var gate = _locks.GetOrAdd(account, _ => new SemaphoreSlim(1, 1));

        await gate.WaitAsync().ConfigureAwait(false);
        try
        {
            // The lock spans the READ as well as the write. Taking it only around the
            // write is the classic near-miss: two callers both read 0, both wait
            // politely for the lock, and both write 1.
            var current = balances.Read(account);

            if (betweenReadAndWrite is not null)
                await betweenReadAndWrite().ConfigureAwait(false);

            balances.Write(account, current + amount);
        }
        finally
        {
            // finally, not after the write. An increment that throws must not leave the
            // account locked for the lifetime of the process.
            gate.Release();
        }
    }
}
