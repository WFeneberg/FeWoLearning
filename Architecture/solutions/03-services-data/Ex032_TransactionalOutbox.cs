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

// Exercise 032 — TransactionalOutbox (reference solution).
public static class Ex032_TransactionalOutbox
{
    public static void PlaceOrder(
        Func<DbConnection> openConnection, IMessageBus bus, string orderId, decimal amount, Action? interrupt = null)
    {
        using var connection = openConnection();
        using var transaction = connection.BeginTransaction();

        using (var insertOrder = connection.CreateCommand())
        {
            insertOrder.Transaction = transaction;
            insertOrder.CommandText = "INSERT INTO orders (id, amount) VALUES (@id, @amount)";
            AddParameter(insertOrder, "@id", orderId);
            AddParameter(insertOrder, "@amount", amount.ToString(System.Globalization.CultureInfo.InvariantCulture));
            insertOrder.ExecuteNonQuery();
        }

        // The crash seam sits here, BETWEEN the two writes. Because both are inside one
        // transaction, throwing here rolls the order back as well - which is the entire
        // promise. Two separate transactions would leave an order nobody is ever told
        // about.
        interrupt?.Invoke();

        using (var insertOutbox = connection.CreateCommand())
        {
            insertOutbox.Transaction = transaction;
            insertOutbox.CommandText =
                "INSERT INTO outbox (type, payload, sent) VALUES (@type, @payload, 0)";
            AddParameter(insertOutbox, "@type", "order.placed");
            AddParameter(insertOutbox, "@payload", orderId);
            insertOutbox.ExecuteNonQuery();
        }

        transaction.Commit();

        // `bus` is in scope and is deliberately never touched. That is the whole
        // discipline: no bus.Publish here.
        _ = bus;

        // Note what does NOT happen here: no bus.Publish. Publishing from inside this
        // method puts back the failure the pattern removes - the publish succeeds, the
        // commit fails, and the rest of the system believes in an order the database
        // has never heard of.
    }

    public static int Relay(Func<DbConnection> openConnection, IMessageBus bus)
    {
        using var connection = openConnection();

        var pending = new List<(long Id, string Type, string Payload)>();

        using (var select = connection.CreateCommand())
        {
            // "WHERE sent = 0", not "SELECT *". A relay that reads every row republishes
            // the whole history on every pass.
            select.CommandText = "SELECT id, type, payload FROM outbox WHERE sent = 0 ORDER BY id";
            using var reader = select.ExecuteReader();
            while (reader.Read())
                pending.Add((Convert.ToInt64(reader.GetValue(0)), reader.GetString(1), reader.GetString(2)));
        }

        foreach (var message in pending)
        {
            bus.Publish(message.Type, message.Payload);

            // Marked sent AFTER a successful publish. The other order would lose a
            // message on a crash in between; this one can deliver it twice, which is why
            // consumers must be idempotent - see exercise 033.
            using var markSent = connection.CreateCommand();
            markSent.CommandText = "UPDATE outbox SET sent = 1 WHERE id = @id";
            AddParameter(markSent, "@id", message.Id);
            markSent.ExecuteNonQuery();
        }

        return pending.Count;
    }

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
