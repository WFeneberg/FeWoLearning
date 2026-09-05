using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex029;

public sealed record Customer(int Id, string Name);

public interface ICustomerRepository
{
    void Add(Customer customer);

    Customer? Find(int id);
}

// Exercise 029 — RepositoryUnitOfWork (services-data).
// Goal:   Make the transaction boundary a first-class object, so a whole operation
//         commits once or not at all.
// Drills: repository boundary, unit of work, single commit per operation.
// Passes: before Commit  - a SECOND connection cannot see the row.
//         after Commit   - it can.
//         read-your-writes - the repository itself CAN see its own uncommitted row.
//         no Commit      - disposing without committing leaves nothing behind.
//         two adds       - one Commit publishes both.
//         double Commit  - refused.
//
// Everything here is graded from a second connection, and that is the only honest way
// to do it. A repository that opens its own connection per call, writes, and closes -
// the "just save it" implementation - satisfies every assertion made through the
// repository's own Find, and has no transaction at all: the first row is durable before
// the second one is even attempted, so a failure halfway through leaves the database
// holding half an operation.
//
// The connection and the transaction live on the UNIT OF WORK, not on the repository.
// That is what makes "one commit per operation" expressible.
public sealed class UnitOfWork : IDisposable
{
    public UnitOfWork(string connectionString) =>
        throw new NotImplementedException(
            "TODO: Ex029 - open a connection, begin a transaction, and hand both to the repository");

    public ICustomerRepository Customers =>
        throw new NotImplementedException("TODO: Ex029 - the repository working inside this transaction");

    public void Commit() =>
        throw new NotImplementedException(
            "TODO: Ex029 - commit the transaction, and refuse a second commit");

    /// <summary>Rolls back if Commit was never called.</summary>
    public void Dispose() =>
        throw new NotImplementedException("TODO: Ex029 - roll back an uncommitted transaction and close the connection");

    /// <summary>Test setup helper: creates the table on its own connection.</summary>
    public static void EnsureCreated(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS customers (id INTEGER PRIMARY KEY, name TEXT NOT NULL);";
        command.ExecuteNonQuery();
    }

    /// <summary>Test helper: reads through a completely separate connection.</summary>
    public static Customer? ReadFromAnotherConnection(string connectionString, int id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM customers WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? new Customer(reader.GetInt32(0), reader.GetString(1)) : null;
    }
}
