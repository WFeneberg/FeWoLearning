using System.Data.Common;
using FeWoLearning.Architecture.Exercises.Evolution.Ex074;
using FeWoLearning.Architecture.Tests.Harness;
using Microsoft.Data.Sqlite;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex074_ExpandContractMigrationTests : IDisposable
{
    private readonly SqliteScratch _scratch = new();
    private readonly SqliteConnection _connection;

    public Ex074_ExpandContractMigrationTests()
    {
        _connection = _scratch.OpenConnection();

        using var create = _connection.CreateCommand();
        create.CommandText = """
            CREATE TABLE users (id TEXT PRIMARY KEY, name TEXT NOT NULL);
            INSERT INTO users (id, name) VALUES ('u-1', 'Ada Lovelace');
            INSERT INTO users (id, name) VALUES ('u-2', 'Grace Hopper');
            """;
        create.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _scratch.Dispose();
    }

    private string? OldColumn(string id)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM users WHERE id = @id";
        Ex074_ExpandContractMigration.AddParameter(command, "@id", id);
        return command.ExecuteScalar() as string;
    }

    private string? NewColumn(string id)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT display_name FROM users WHERE id = @id";
        Ex074_ExpandContractMigration.AddParameter(command, "@id", id);
        return command.ExecuteScalar() as string;
    }

    [Fact]
    public void Expand_Leaves_The_Old_Column_Alone()
    {
        // Old code carries on reading and writing `name` and never notices. A migration
        // that renames in place is correct for exactly as long as the deployment takes -
        // and a deployment is never atomic.
        Ex074_ExpandContractMigration.Expand(_connection);

        Assert.Equal("Ada Lovelace", OldColumn("u-1"));
        Assert.Null(NewColumn("u-1"));
    }

    [Fact]
    public void Mechanism_A_Write_During_The_Migration_Lands_In_Both_Columns()
    {
        // The entire technique. A writer that fills only the new column leaves every
        // instance still running the old code reading a null it has no case for - and
        // those instances exist, for the length of the deployment, by definition.
        Ex074_ExpandContractMigration.Expand(_connection);

        Ex074_ExpandContractMigration.WriteDuringMigration(_connection, "u-3", "Katherine Johnson");

        Assert.Equal("Katherine Johnson", OldColumn("u-3"));
        Assert.Equal("Katherine Johnson", NewColumn("u-3"));
    }

    [Fact]
    public void Mechanism_Reading_During_The_Migration_Handles_Rows_From_Both_Eras()
    {
        // A row written before Expand has no display_name yet, and the new code must not
        // treat that as a missing user. Reading only the new column is the mistake that
        // makes half the users disappear the moment the new build ships.
        Ex074_ExpandContractMigration.Expand(_connection);
        Ex074_ExpandContractMigration.WriteDuringMigration(_connection, "u-3", "Katherine Johnson");

        Assert.Equal("Ada Lovelace", Ex074_ExpandContractMigration.ReadDuringMigration(_connection, "u-1"));
        Assert.Equal("Katherine Johnson", Ex074_ExpandContractMigration.ReadDuringMigration(_connection, "u-3"));
    }

    [Fact]
    public void Backfill_Fills_The_Rows_That_Predate_Expand()
    {
        Ex074_ExpandContractMigration.Expand(_connection);
        Assert.Equal(2, Ex074_ExpandContractMigration.RowsMissingDisplayName(_connection));

        var changed = Ex074_ExpandContractMigration.Backfill(_connection);

        Assert.Equal(2, changed);
        Assert.Equal(0, Ex074_ExpandContractMigration.RowsMissingDisplayName(_connection));
    }

    [Fact]
    public void Adversarial_Backfill_Does_Not_Overwrite_What_The_New_Code_Already_Wrote()
    {
        // A backfill that copies unconditionally undoes every edit made since Expand - and
        // it looks correct afterwards, because every row has a value. Re-runnability and
        // not-clobbering are the same clause.
        Ex074_ExpandContractMigration.Expand(_connection);

        using (var edit = _connection.CreateCommand())
        {
            edit.CommandText = "UPDATE users SET display_name = 'Ada L.' WHERE id = 'u-1'";
            edit.ExecuteNonQuery();
        }

        var changed = Ex074_ExpandContractMigration.Backfill(_connection);

        Assert.Equal(1, changed);              // only u-2 needed it
        Assert.Equal("Ada L.", NewColumn("u-1"));

        // ...and running it again changes nothing at all.
        Assert.Equal(0, Ex074_ExpandContractMigration.Backfill(_connection));
        Assert.Equal("Ada L.", NewColumn("u-1"));
    }

    [Fact]
    public void Contract_Drops_The_Old_Column_And_Reads_Still_Work()
    {
        Ex074_ExpandContractMigration.Expand(_connection);
        Ex074_ExpandContractMigration.Backfill(_connection);

        Ex074_ExpandContractMigration.Contract(_connection);

        Assert.Equal("Ada Lovelace", Ex074_ExpandContractMigration.ReadAfterContract(_connection, "u-1"));
        Assert.Throws<SqliteException>(() => OldColumn("u-1"));
    }

    [Fact]
    public async Task Container_The_Whole_Migration_Runs_On_Real_Postgres()
    {
        // SQLite has supported DROP COLUMN since 3.35 and is fine for the shape of this;
        // Postgres is where the phases actually earn their keep, with real locks and real
        // concurrent readers. This runs the exercise's own migration steps against it.
        // Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();

        await using DbConnection connection = new NpgsqlConnection(postgres.GetConnectionString());
        await connection.OpenAsync();

        await using (var create = connection.CreateCommand())
        {
            create.CommandText = """
                CREATE TABLE users (id TEXT PRIMARY KEY, name TEXT NOT NULL);
                INSERT INTO users (id, name) VALUES ('u-1', 'Ada Lovelace');
                """;
            await create.ExecuteNonQueryAsync();
        }

        Ex074_ExpandContractMigration.Expand(connection);
        Ex074_ExpandContractMigration.WriteDuringMigration(connection, "u-2", "Grace Hopper");

        Assert.Equal("Ada Lovelace", Ex074_ExpandContractMigration.ReadDuringMigration(connection, "u-1"));
        Assert.Equal(1, Ex074_ExpandContractMigration.RowsMissingDisplayName(connection));

        Assert.Equal(1, Ex074_ExpandContractMigration.Backfill(connection));
        Assert.Equal(0, Ex074_ExpandContractMigration.RowsMissingDisplayName(connection));

        Ex074_ExpandContractMigration.Contract(connection);

        Assert.Equal("Ada Lovelace", Ex074_ExpandContractMigration.ReadAfterContract(connection, "u-1"));
        Assert.Equal("Grace Hopper", Ex074_ExpandContractMigration.ReadAfterContract(connection, "u-2"));
    }
}
