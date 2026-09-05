using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// A throwaway SQLite database on disk.
///
/// Deliberately a temp FILE and not ":memory:". Several exercises - the outbox, the
/// unit of work, pessimistic locking - prove a transaction boundary by opening a
/// SECOND connection and checking what it can and cannot see. Every ":memory:"
/// connection gets its own private database, which would make those facts pass
/// vacuously.
/// </summary>
public sealed class SqliteScratch : IDisposable
{
    private readonly string _path;

    public SqliteScratch()
    {
        _path = Path.Combine(Path.GetTempPath(),
            "fewo-arch-" + Guid.NewGuid().ToString("N") + ".db");
        ConnectionString = new SqliteConnectionStringBuilder { DataSource = _path }.ToString();
    }

    public string ConnectionString { get; }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public void Execute(string sql)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        // Pooled connections keep the file handle open on Windows; without this the
        // delete throws IOException and every test leaks a database file.
        SqliteConnection.ClearAllPools();
        if (File.Exists(_path)) File.Delete(_path);
    }
}
