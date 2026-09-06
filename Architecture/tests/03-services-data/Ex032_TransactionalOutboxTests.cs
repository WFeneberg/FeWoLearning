using System.Data.Common;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex032;
using FeWoLearning.Architecture.Tests.Harness;
using Microsoft.Data.Sqlite;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex032_TransactionalOutboxTests : IDisposable
{
    private readonly SqliteScratch _scratch = new();
    private readonly Func<DbConnection> _open;

    public Ex032_TransactionalOutboxTests()
    {
        _open = () =>
        {
            var connection = new SqliteConnection(_scratch.ConnectionString);
            connection.Open();
            return connection;
        };

        _scratch.Execute("""
            CREATE TABLE orders (id TEXT PRIMARY KEY, amount TEXT NOT NULL);
            CREATE TABLE outbox (
                id      INTEGER PRIMARY KEY AUTOINCREMENT,
                type    TEXT NOT NULL,
                payload TEXT NOT NULL,
                sent    INTEGER NOT NULL DEFAULT 0
            );
            """);
    }

    public void Dispose() => _scratch.Dispose();

    [Fact]
    public void Placing_An_Order_Writes_The_Row_And_An_Unsent_Outbox_Message()
    {
        Ex032_TransactionalOutbox.PlaceOrder(_open, new RecordingBus(), "O-1", 42m);

        Assert.True(Ex032_TransactionalOutbox.OrderExists(_open, "O-1"));

        var message = Assert.Single(Ex032_TransactionalOutbox.ReadOutbox(_open));
        Assert.Equal("order.placed", message.Type);
        Assert.Equal("O-1", message.Payload);
        Assert.False(message.Sent);
    }

    [Fact]
    public void Mechanism_The_Bus_Has_Seen_Nothing_Before_The_Relay_Runs()
    {
        // What makes this an outbox rather than a table with an unfortunate name.
        // Writing the row AND publishing inside PlaceOrder passes every other assertion
        // in this file and puts back the exact failure the pattern removes: the publish
        // succeeds, the commit fails, and the rest of the system believes in an order
        // the database has never heard of.
        var bus = new RecordingBus();

        Ex032_TransactionalOutbox.PlaceOrder(_open, bus, "O-1", 42m);

        Assert.Empty(bus.Published);
    }

    [Fact]
    public void Mechanism_A_Crash_Between_The_Two_Writes_Leaves_Neither()
    {
        // Atomicity, observed from a separate connection. Two transactions - one per
        // write - produce an order with no outbox row, which is an order nobody will
        // ever be told about.
        Assert.Throws<InvalidOperationException>(() => Ex032_TransactionalOutbox.PlaceOrder(
            _open, new RecordingBus(), "O-1", 42m,
            interrupt: () => throw new InvalidOperationException("the process died here")));

        Assert.False(Ex032_TransactionalOutbox.OrderExists(_open, "O-1"));
        Assert.Empty(Ex032_TransactionalOutbox.ReadOutbox(_open));
    }

    [Fact]
    public void The_Relay_Publishes_Pending_Messages_And_Marks_Them_Sent()
    {
        var bus = new RecordingBus();
        Ex032_TransactionalOutbox.PlaceOrder(_open, bus, "O-1", 42m);
        Ex032_TransactionalOutbox.PlaceOrder(_open, bus, "O-2", 7m);

        var published = Ex032_TransactionalOutbox.Relay(_open, bus);

        Assert.Equal(2, published);
        Assert.Equal([("order.placed", "O-1"), ("order.placed", "O-2")], bus.Published);
        Assert.All(Ex032_TransactionalOutbox.ReadOutbox(_open), m => Assert.True(m.Sent));
    }

    [Fact]
    public void Adversarial_Running_The_Relay_Again_Publishes_Nothing()
    {
        // A relay that reads every row rather than the unsent ones republishes the whole
        // history on every pass, and every consumer is at-least-once, so they will act
        // on it.
        var bus = new RecordingBus();
        Ex032_TransactionalOutbox.PlaceOrder(_open, bus, "O-1", 42m);
        Ex032_TransactionalOutbox.Relay(_open, bus);

        var second = Ex032_TransactionalOutbox.Relay(_open, bus);

        Assert.Equal(0, second);
        Assert.Single(bus.Published);
    }

    [Fact]
    public async Task Container_The_Same_Code_Is_Atomic_On_Real_Postgres()
    {
        // Not a demonstration that Postgres has transactions - this runs the EXERCISE'S
        // OWN PlaceOrder and Relay against it. That is why the exercise takes a
        // Func<DbConnection> and uses "@name" parameters, which Microsoft.Data.Sqlite
        // and Npgsql both accept. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();

        var connectionString = postgres.GetConnectionString();
        Func<DbConnection> open = () =>
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        };

        await using (var setup = new NpgsqlConnection(connectionString))
        {
            await setup.OpenAsync();
            await using var create = new NpgsqlCommand("""
                CREATE TABLE orders (id TEXT PRIMARY KEY, amount TEXT NOT NULL);
                CREATE TABLE outbox (
                    id      BIGSERIAL PRIMARY KEY,
                    type    TEXT NOT NULL,
                    payload TEXT NOT NULL,
                    sent    INTEGER NOT NULL DEFAULT 0
                );
                """, setup);
            await create.ExecuteNonQueryAsync();
        }

        // The happy path, then the crash - both through the exercise.
        var bus = new RecordingBus();
        Ex032_TransactionalOutbox.PlaceOrder(open, bus, "O-1", 42m);

        Assert.Throws<InvalidOperationException>(() => Ex032_TransactionalOutbox.PlaceOrder(
            open, bus, "O-2", 7m, interrupt: () => throw new InvalidOperationException("crash")));

        Assert.True(Ex032_TransactionalOutbox.OrderExists(open, "O-1"));
        Assert.False(Ex032_TransactionalOutbox.OrderExists(open, "O-2"));

        Assert.Empty(bus.Published); // PlaceOrder had the bus and did not use it
        Assert.Equal(1, Ex032_TransactionalOutbox.Relay(open, bus));
        Assert.Equal([("order.placed", "O-1")], bus.Published);
        Assert.Equal(0, Ex032_TransactionalOutbox.Relay(open, bus));
    }
}
