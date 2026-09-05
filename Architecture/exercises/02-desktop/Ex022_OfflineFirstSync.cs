using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex022;

/// <summary>
/// A note as the device holds it. BaseVersion is the server version this copy was made
/// from - not the version it will become. Without it, "the server moved on while I was
/// offline" is indistinguishable from "the server has not seen my change yet".
/// </summary>
public sealed record LocalNote(string Id, string Text, int BaseVersion, bool IsDirty);

public sealed record ServerNote(string Id, string Text, int Version);

/// <summary>Both texts, so the losing edit is recoverable rather than merely gone.</summary>
public sealed record Conflict(string Id, string LocalText, string ServerText);

public sealed record SyncResult(int Pushed, int Pulled, IReadOnlyList<Conflict> Conflicts);

/// <summary>
/// Real SQLite, given whole - the exercise is the sync policy, not the SQL. It is a
/// file database, so a second LocalStore over the same connection string is what a
/// restart of the application looks like.
/// </summary>
public sealed class LocalStore(string connectionString)
{
    public void EnsureCreated()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS notes (
                id           TEXT PRIMARY KEY,
                text         TEXT NOT NULL,
                base_version INTEGER NOT NULL,
                is_dirty     INTEGER NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public void Upsert(LocalNote note)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO notes (id, text, base_version, is_dirty)
            VALUES ($id, $text, $version, $dirty)
            ON CONFLICT(id) DO UPDATE SET
                text = excluded.text,
                base_version = excluded.base_version,
                is_dirty = excluded.is_dirty;
            """;
        command.Parameters.AddWithValue("$id", note.Id);
        command.Parameters.AddWithValue("$text", note.Text);
        command.Parameters.AddWithValue("$version", note.BaseVersion);
        command.Parameters.AddWithValue("$dirty", note.IsDirty ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public LocalNote? Find(string id) => All().FirstOrDefault(n => n.Id == id);

    public IReadOnlyList<LocalNote> All()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, text, base_version, is_dirty FROM notes ORDER BY id";

        var notes = new List<LocalNote>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            notes.Add(new LocalNote(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3) != 0));

        return notes;
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}

// Exercise 022 — OfflineFirstSync (desktop).
// Goal:   Reconcile a local store with a server snapshot, deciding per note who wins
//         and never losing an edit silently.
// Drills: local SQLite store, change tracking, conflict resolution policy.
// Passes: new on server        - inserted locally, clean, counted as Pulled.
//         clean and stale      - overwritten from the server, counted as Pulled.
//         clean and current    - untouched; Pulled stays 0.
//         dirty, server agrees - the local text is pushed: the note becomes clean at
//                                BaseVersion + 1 and is counted as Pushed.
//         dirty, server moved  - a CONFLICT. The server text wins locally, and the
//                                Conflict entry carries BOTH texts.
//         local-only and dirty - pushed as new.
//         everything           - is written THROUGH the store, so a second LocalStore
//                                over the same file sees it.
//
// The conflict entry is the fact worth caring about. "Last write wins" is a perfectly
// ordinary implementation that passes every count above and quietly destroys work
// somebody did on a train with no signal. Losing the edit is sometimes acceptable;
// losing it without saying so never is.
public static class Ex022_OfflineFirstSync
{
    public static SyncResult Sync(LocalStore local, IReadOnlyList<ServerNote> server) =>
        throw new NotImplementedException(
            "TODO: Ex022 - reconcile each note by BaseVersion and IsDirty, recording both texts whenever the server moved on under a dirty local edit");
}
