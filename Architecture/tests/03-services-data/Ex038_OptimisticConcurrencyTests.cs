using System.Data.Common;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex038;
using FeWoLearning.Architecture.Tests.Harness;
using Microsoft.Data.Sqlite;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex038_OptimisticConcurrencyTests : IDisposable
{
    private readonly SqliteScratch _scratch = new();
    private readonly VersionedDocumentStore _store;

    public Ex038_OptimisticConcurrencyTests()
    {
        _scratch.Execute(
            "CREATE TABLE documents (id TEXT PRIMARY KEY, text TEXT NOT NULL, version INTEGER NOT NULL);");

        _store = new VersionedDocumentStore(() =>
        {
            var connection = new SqliteConnection(_scratch.ConnectionString);
            connection.Open();
            return connection;
        });

        _store.Insert(new Document("d-1", "draft", 1));
    }

    public void Dispose() => _scratch.Dispose();

    [Fact]
    public void An_Update_From_The_Current_Version_Succeeds_And_Moves_It_On()
    {
        var current = _store.Read("d-1")!;

        Assert.True(_store.TryUpdate(current, "reviewed"));

        var after = _store.Read("d-1")!;
        Assert.Equal("reviewed", after.Text);
        Assert.Equal(2, after.Version);
    }

    [Fact]
    public void Mechanism_The_Second_Of_Two_Writers_Is_Refused()
    {
        // Two readers, one row, both at version 1 - which is the ordinary shape of two
        // people opening the same record. Without the version in the WHERE clause the
        // second write simply lands, and the first person's edit is gone with nothing
        // anywhere recording that it ever existed.
        var alice = _store.Read("d-1")!;
        var bob = _store.Read("d-1")!;

        Assert.True(_store.TryUpdate(alice, "alice's edit"));
        Assert.False(_store.TryUpdate(bob, "bob's edit"));
    }

    [Fact]
    public void Mechanism_The_Row_Still_Holds_The_Winners_Value()
    {
        // Returning false is not enough on its own: an implementation could report the
        // conflict and write anyway.
        var alice = _store.Read("d-1")!;
        var bob = _store.Read("d-1")!;

        _store.TryUpdate(alice, "alice's edit");
        _store.TryUpdate(bob, "bob's edit");

        var after = _store.Read("d-1")!;
        Assert.Equal("alice's edit", after.Text);
        Assert.Equal(2, after.Version);
    }

    [Fact]
    public void Adversarial_A_Stale_Writer_Is_Refused_After_A_Textually_Empty_Edit()
    {
        // What separates a version column from a compare-and-swap on the CONTENT.
        // Comparing the stored text against what the writer expected to find behaves
        // identically to a version check in every scenario where the text changed - which
        // is why the first draft of this fact, written that way, let the wrong
        // implementation through completely. Measured.
        //
        // The two designs part company exactly here: Alice re-saves the same text - a
        // reformat, a whitespace normalisation, a touch to bump a timestamp - so the row
        // moves to version 2 with its text unchanged. Bob, still holding version 1, is
        // now working from a state that no longer exists. The version knows; the content
        // is identical and cannot.
        var alice = _store.Read("d-1")!;
        var bob = _store.Read("d-1")!;

        Assert.True(_store.TryUpdate(alice, "draft"));
        Assert.Equal(2, _store.Read("d-1")!.Version);

        Assert.False(_store.TryUpdate(bob, "bob's edit"));
        Assert.Equal("draft", _store.Read("d-1")!.Text);
    }

    [Fact]
    public void Reloading_And_Retrying_Succeeds()
    {
        // The other half of the contract: a conflict is recoverable, not fatal.
        var alice = _store.Read("d-1")!;
        var bob = _store.Read("d-1")!;
        _store.TryUpdate(alice, "alice's edit");

        Assert.False(_store.TryUpdate(bob, "bob's edit"));

        var reloaded = _store.Read("d-1")!;
        Assert.True(_store.TryUpdate(reloaded, "bob's edit, on top of alice's"));
        Assert.Equal(3, _store.Read("d-1")!.Version);
    }

    [Fact]
    public async Task Container_The_Same_Code_Detects_The_Conflict_On_Real_Postgres()
    {
        // Runs the exercise's own TryUpdate against a database with real concurrent
        // writers and a real isolation level, rather than asserting that Postgres has
        // an UPDATE statement. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync();

        var connectionString = postgres.GetConnectionString();

        await using (var setup = new NpgsqlConnection(connectionString))
        {
            await setup.OpenAsync();
            await using var create = new NpgsqlCommand(
                "CREATE TABLE documents (id TEXT PRIMARY KEY, text TEXT NOT NULL, version INTEGER NOT NULL);", setup);
            await create.ExecuteNonQueryAsync();
        }

        var store = new VersionedDocumentStore(() =>
        {
            DbConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        });

        store.Insert(new Document("d-1", "draft", 1));

        var alice = store.Read("d-1")!;
        var bob = store.Read("d-1")!;

        Assert.True(store.TryUpdate(alice, "alice's edit"));
        Assert.False(store.TryUpdate(bob, "bob's edit"));
        Assert.Equal("alice's edit", store.Read("d-1")!.Text);
    }
}
