using FeWoLearning.Architecture.Exercises.Runtime.Ex092;
using FeWoLearning.Architecture.Exercises.Support;
using FeWoLearning.Architecture.Tests.Harness;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Architecture.Tests.Runtime;

public class Ex092_ConnectionPoolingTests
{
    private static (ConnectionPool Pool, ManualClock Clock) Build(int size = 2)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new ConnectionPool(clock, size), clock);
    }

    [Fact]
    public void Mechanism_A_Returned_Connection_Is_Reused()
    {
        // A pool that hands out a new one every time is a factory with extra steps - and
        // the thing it is pooling costs a TCP connection and a TLS handshake to make.
        var (pool, _) = Build();

        var first = pool.Lease();
        pool.Return(first);
        var second = pool.Lease();

        Assert.Same(first, second);
        Assert.Equal(2, second.Uses);
    }

    [Fact]
    public void Only_As_Many_Connections_Exist_As_The_Pool_Was_Given()
    {
        var (pool, _) = Build(size: 2);

        var a = pool.Lease();
        var b = pool.Lease();

        Assert.NotSame(a, b);
        Assert.Equal(2, pool.InUse);
        Assert.Equal(0, pool.Available);
    }

    [Fact]
    public void Mechanism_An_Exhausted_Pool_Fails_Rather_Than_Waiting()
    {
        // A caller blocked on an exhausted pool holds its own request thread while it
        // waits, so an exhausted pool becomes an exhausted thread pool - and the outage
        // spreads to endpoints that never touched the database.
        var (pool, _) = Build(size: 2);
        pool.Lease();
        pool.Lease();

        var failure = Assert.Throws<PoolExhaustedException>(pool.Lease);

        Assert.Equal(2, failure.Size);
    }

    [Fact]
    public void Returning_Makes_A_Connection_Available_Again()
    {
        var (pool, _) = Build(size: 1);
        var connection = pool.Lease();

        pool.Return(connection);

        Assert.Equal(1, pool.Available);
        Assert.Null(Record.Exception(() => pool.Lease()));
    }

    [Fact]
    public void Adversarial_Returning_Twice_Does_Not_Create_Capacity()
    {
        // The mirror-image bug, and the worse one because it does not look like a bug: a
        // double return puts one connection in the pool twice, two callers lease the same
        // one, and their queries interleave on a single session.
        var (pool, _) = Build(size: 1);
        var connection = pool.Lease();

        pool.Return(connection);
        pool.Return(connection);

        Assert.Equal(1, pool.Available);

        pool.Lease();
        Assert.Throws<PoolExhaustedException>(pool.Lease);
    }

    [Fact]
    public void Mechanism_A_Leaked_Connection_Is_Reported_With_Its_Age()
    {
        // "Exhausted" tells you the pool is broken; it does not tell you who broke it, and
        // the answer is always a code path that forgot a `using`. The pool does not grow,
        // so every leak is permanent and the symptom arrives hours later somewhere
        // unrelated.
        var (pool, clock) = Build(size: 3);
        var leaked = pool.Lease();
        clock.Advance(TimeSpan.FromMinutes(10));
        var healthy = pool.Lease();

        var leaks = pool.FindLeaks(TimeSpan.FromMinutes(5));

        Assert.Equal([leaked.Id], leaks.Select(c => c.Id));
        Assert.DoesNotContain(healthy.Id, leaks.Select(c => c.Id));
    }

    [Fact]
    public void A_Pool_Where_Everything_Came_Back_Reports_No_Leaks()
    {
        // Paired with the fact above - alone, "return an empty list" satisfies it.
        var (pool, clock) = Build(size: 3);
        var connection = pool.Lease();
        pool.Return(connection);

        clock.Advance(TimeSpan.FromHours(1));

        Assert.Empty(pool.FindLeaks(TimeSpan.FromMinutes(5)));
    }

    [Fact]
    public async Task Container_A_Real_Postgres_Pool_Refuses_Rather_Than_Hanging_For_Ever()
    {
        // The in-process facts grade the policy. This one checks the assumption underneath
        // it: a real driver pool with a bounded size and a timeout FAILS when it is
        // exhausted, rather than blocking indefinitely - which is what makes "fail fast"
        // an available choice at all. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();

        var connectionString = new NpgsqlConnectionStringBuilder(postgres.GetConnectionString())
        {
            MaxPoolSize = 2,
            Timeout = 2,
        }.ToString();

        await using var first = new NpgsqlConnection(connectionString);
        await using var second = new NpgsqlConnection(connectionString);
        await first.OpenAsync();
        await second.OpenAsync();

        await using var third = new NpgsqlConnection(connectionString);

        await Assert.ThrowsAnyAsync<Exception>(() => third.OpenAsync());

        // ...and returning one makes the pool usable again, which is the other half.
        await first.CloseAsync();
        await using var fourth = new NpgsqlConnection(connectionString);
        await fourth.OpenAsync();

        Assert.Equal(System.Data.ConnectionState.Open, fourth.State);
    }
}
