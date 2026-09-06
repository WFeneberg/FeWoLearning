using FeWoLearning.Architecture.Exercises.ServicesData.Ex039;
using FeWoLearning.Architecture.Tests.Harness;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex039_PessimisticLockingTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(15);

    /// <summary>Same fail-fast gate wait as Ex036 - a throwing stub must not sit out the timeout.</summary>
    private static void WaitForArrival(CountdownEvent arrived, params Task[] racers)
    {
        var deadline = DateTime.UtcNow + Patience;

        while (!arrived.Wait(TimeSpan.FromMilliseconds(25)))
        {
            foreach (var racer in racers)
                if (racer.IsFaulted)
                    racer.GetAwaiter().GetResult();

            Assert.True(DateTime.UtcNow < deadline, "the increments never reached the gate");
        }
    }

    [Fact]
    public async Task Sequential_Increments_Accumulate()
    {
        var balances = new Balances();
        var locking = new KeyedLockingBalances(balances);

        await locking.IncrementAsync("acc", 1);
        await locking.IncrementAsync("acc", 1);

        Assert.Equal(2, balances.Read("acc"));
    }

    [Fact]
    public async Task Mechanism_Concurrent_Increments_Of_One_Account_All_Land()
    {
        // The suspension point between read and write is what makes this deterministic:
        // without the lock, every caller reads 0, yields, and writes 1, so the final
        // balance is 1 rather than 8 - every time, not just on an unlucky machine.
        const int callers = 8;
        var balances = new Balances();
        var locking = new KeyedLockingBalances(balances);

        var increments = Enumerable.Range(0, callers)
            .Select(_ => Task.Run(() => locking.IncrementAsync("acc", 1, async () => await Task.Yield())))
            .ToArray();

        await Task.WhenAll(increments).WaitAsync(Patience);

        Assert.Equal(callers, balances.Read("acc"));
    }

    [Fact]
    public async Task Mechanism_Two_Different_Accounts_Are_Not_Serialised_Against_Each_Other()
    {
        // Catches the single global lock, which makes every increment correct and every
        // unrelated account queue behind the slowest one - a table lock wearing a row
        // lock's name. Both must be inside their critical sections at the same time, so
        // a global lock cannot reach the gate.
        var balances = new Balances();
        var locking = new KeyedLockingBalances(balances);
        using var bothInside = new CountdownEvent(2);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task Hold() { bothInside.Signal(); return release.Task; }

        var a = Task.Run(() => locking.IncrementAsync("acc-a", 1, Hold));
        var b = Task.Run(() => locking.IncrementAsync("acc-b", 1, Hold));

        WaitForArrival(bothInside, a, b);
        release.SetResult();

        await Task.WhenAll(a, b).WaitAsync(Patience);

        Assert.Equal(1, balances.Read("acc-a"));
        Assert.Equal(1, balances.Read("acc-b"));
    }

    [Fact]
    public async Task Adversarial_A_Failed_Increment_Still_Releases_The_Account()
    {
        // Releasing after the write rather than in a finally leaves the account locked
        // for the lifetime of the process the first time anything throws - and the next
        // caller does not fail, it hangs.
        var balances = new Balances();
        var locking = new KeyedLockingBalances(balances);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            locking.IncrementAsync("acc", 1, () => throw new InvalidOperationException("boom")));

        await locking.IncrementAsync("acc", 5).WaitAsync(Patience);

        Assert.Equal(5, balances.Read("acc"));
    }

    [Fact]
    public async Task Container_Real_Postgres_Serialises_Two_Writers_With_Select_For_Update()
    {
        // The in-process facts prove the policy. This one proves the database primitive
        // it is normally delegated to: with SELECT ... FOR UPDATE the second transaction
        // blocks until the first commits, so both increments land. Skipped unless
        // -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();
        var connectionString = postgres.GetConnectionString();

        await using (var setup = new NpgsqlConnection(connectionString))
        {
            await setup.OpenAsync();
            await using var create = new NpgsqlCommand(
                "CREATE TABLE balances (account TEXT PRIMARY KEY, value INTEGER NOT NULL); " +
                "INSERT INTO balances VALUES ('acc', 0);", setup);
            await create.ExecuteNonQueryAsync();
        }

        async Task Increment()
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            await using var read = new NpgsqlCommand(
                "SELECT value FROM balances WHERE account = 'acc' FOR UPDATE", connection, transaction);
            var current = (int)(await read.ExecuteScalarAsync())!;

            await Task.Delay(50); // hold the lock long enough for the other writer to queue

            await using var write = new NpgsqlCommand(
                "UPDATE balances SET value = @v WHERE account = 'acc'", connection, transaction);
            write.Parameters.AddWithValue("v", current + 1);
            await write.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }

        await Task.WhenAll(Increment(), Increment());

        await using var verify = new NpgsqlConnection(connectionString);
        await verify.OpenAsync();
        await using var total = new NpgsqlCommand("SELECT value FROM balances WHERE account = 'acc'", verify);

        Assert.Equal(2, (int)(await total.ExecuteScalarAsync())!);
    }
}
