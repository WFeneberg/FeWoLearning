using System.Data.Common;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex032;

public sealed record OutboxMessage(long Id, string Type, string Payload, bool Sent);

public interface IMessageBus
{
    void Publish(string type, string payload);
}

/// <summary>Records everything it was told to publish, so "nothing yet" is checkable.</summary>
public sealed class RecordingBus : IMessageBus
{
    public List<(string Type, string Payload)> Published { get; } = [];

    public void Publish(string type, string payload) => Published.Add((type, payload));
}

// Exercise 032 — TransactionalOutbox (services-data).
// Goal:   Make "the order was stored" and "the world was told about it" a single
//         all-or-nothing fact, without a distributed transaction.
// Drills: same-transaction persist and enqueue, relay, rollback atomicity.
// Passes: PlaceOrder - writes the order row AND an unsent outbox row in ONE transaction.
//         atomicity  - if something throws between the two writes, NEITHER row exists.
//         THE ONE     - after PlaceOrder, the BUS HAS SEEN NOTHING. Publishing is the
//                       relay's job and happens later, from the committed row.
//         Relay      - publishes every unsent message and marks it sent; running it a
//                       second time publishes nothing again.
//
// "The bus has seen nothing yet" is what makes this an outbox rather than a table with
// an unfortunate name. Writing the row AND publishing inside PlaceOrder passes every
// other assertion here and reintroduces the exact failure the pattern exists to remove:
// the publish succeeds, the commit fails, and the rest of the system now believes in an
// order the database has never heard of. The reverse ordering is no better - commit,
// then crash before publishing, and the order exists and nobody is told.
//
// The relay's own delivery is at-least-once by construction: it can publish and then
// fail before marking the row sent. That is deliberate, and it is why exercise 033
// exists.
//
// Everything takes a Func<DbConnection> rather than a connection string, and every
// parameter uses the "@name" prefix, which Microsoft.Data.Sqlite and Npgsql both accept.
// That is what lets the container fact run THIS code against real Postgres instead of
// merely demonstrating that Postgres has transactions.
public static class Ex032_TransactionalOutbox
{
    /// <summary>
    /// Store the order and enqueue an "order.placed" outbox message carrying the order
    /// id as its payload - both inside one transaction, neither published.
    /// <paramref name="interrupt"/> runs BETWEEN the two writes; the tests use it to
    /// simulate a crash there.
    ///
    /// <paramref name="bus"/> is handed in ON PURPOSE and must NOT be used. It is here
    /// so that publishing from this method is something you could actually write - which
    /// is what makes the fact that forbids it worth anything.
    /// </summary>
    public static void PlaceOrder(
        Func<DbConnection> openConnection, IMessageBus bus, string orderId, decimal amount, Action? interrupt = null) =>
        throw new NotImplementedException(
            "TODO: Ex032 - insert the order and the unsent outbox row in one transaction, calling interrupt between them, and publish nothing");

    /// <summary>
    /// Publish every unsent outbox message, oldest first, and mark each one sent.
    /// Returns how many were published.
    /// </summary>
    public static int Relay(Func<DbConnection> openConnection, IMessageBus bus) =>
        throw new NotImplementedException(
            "TODO: Ex032 - read the unsent rows in id order, publish each one, and mark it sent");

    /// <summary>Shared helper: adds a parameter without naming a provider.</summary>
    public static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    /// <summary>Test helper: reads the order table on its own connection.</summary>
    public static bool OrderExists(Func<DbConnection> openConnection, string orderId)
    {
        using var connection = openConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM orders WHERE id = @id";
        AddParameter(command, "@id", orderId);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    /// <summary>Test helper: reads the outbox on its own connection.</summary>
    public static IReadOnlyList<OutboxMessage> ReadOutbox(Func<DbConnection> openConnection)
    {
        using var connection = openConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, type, payload, sent FROM outbox ORDER BY id";

        var messages = new List<OutboxMessage>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            messages.Add(new OutboxMessage(
                Convert.ToInt64(reader.GetValue(0)),
                reader.GetString(1),
                reader.GetString(2),
                Convert.ToInt32(reader.GetValue(3)) != 0));

        return messages;
    }
}
