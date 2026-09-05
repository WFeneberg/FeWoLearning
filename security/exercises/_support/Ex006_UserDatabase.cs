using Microsoft.Data.Sqlite;

namespace FeWoLearning.Security.Exercises.Support;

// An in-memory SQLite database seeded with two users. Row 006 is the only row that
// needs a real database: SQL injection cannot be honestly proven against a fake,
// because a test that merely inspects command text is satisfied by a solution that
// builds a right-looking string and concatenates somewhere else.
public sealed class Ex006_UserDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public Ex006_UserDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var seed = _connection.CreateCommand();
        seed.CommandText =
            "create table users (id integer primary key, name text not null, email text not null);" +
            "insert into users (id, name, email) values (1, 'ada', 'ada@example.com'), (2, 'bob', 'bob@example.com');";
        seed.ExecuteNonQuery();
    }

    public SqliteConnection Connection => _connection;

    public void Dispose() => _connection.Dispose();
}
