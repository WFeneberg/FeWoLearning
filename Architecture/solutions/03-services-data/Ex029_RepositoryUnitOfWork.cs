using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex029;

public sealed record Customer(int Id, string Name);

public interface ICustomerRepository
{
    void Add(Customer customer);

    Customer? Find(int id);
}

// Exercise 029 — RepositoryUnitOfWork (reference solution).
public sealed class UnitOfWork : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SqliteTransaction _transaction;
    private bool _committed;

    public UnitOfWork(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        // The transaction belongs to the unit of work, not to the repository. That is
        // exactly what makes "one commit per operation" something you can express: the
        // repository has no way to commit even if it wanted to.
        _transaction = _connection.BeginTransaction();

        Customers = new SqliteCustomerRepository(_connection, _transaction);
    }

    public ICustomerRepository Customers { get; }

    public void Commit()
    {
        if (_committed)
            throw new InvalidOperationException("This unit of work has already been committed.");

        _transaction.Commit();
        _committed = true;
    }

    public void Dispose()
    {
        // Rollback is the DEFAULT. An operation that threw halfway through leaves
        // nothing behind, because nobody reached the Commit call.
        if (!_committed)
            _transaction.Rollback();

        _transaction.Dispose();
        _connection.Dispose();
    }

    public static void EnsureCreated(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS customers (id INTEGER PRIMARY KEY, name TEXT NOT NULL);";
        command.ExecuteNonQuery();
    }

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

    /// <summary>
    /// Takes the connection AND the transaction it must enlist in. A repository that
    /// opened its own connection would be writing outside the unit of work entirely,
    /// and every assertion made through its own Find would still pass.
    /// </summary>
    private sealed class SqliteCustomerRepository(SqliteConnection connection, SqliteTransaction transaction)
        : ICustomerRepository
    {
        public void Add(Customer customer)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO customers (id, name) VALUES ($id, $name)";
            command.Parameters.AddWithValue("$id", customer.Id);
            command.Parameters.AddWithValue("$name", customer.Name);
            command.ExecuteNonQuery();
        }

        public Customer? Find(int id)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "SELECT id, name FROM customers WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? new Customer(reader.GetInt32(0), reader.GetString(1)) : null;
        }
    }
}
