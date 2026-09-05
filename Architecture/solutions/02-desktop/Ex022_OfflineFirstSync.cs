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

// Exercise 022 — OfflineFirstSync (reference solution).
public static class Ex022_OfflineFirstSync
{
    public static SyncResult Sync(LocalStore local, IReadOnlyList<ServerNote> server)
    {
        var pushed = 0;
        var pulled = 0;
        var conflicts = new List<Conflict>();

        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var remote in server)
        {
            seen.Add(remote.Id);
            var mine = local.Find(remote.Id);

            if (mine is null)
            {
                local.Upsert(new LocalNote(remote.Id, remote.Text, remote.Version, IsDirty: false));
                pulled++;
                continue;
            }

            if (!mine.IsDirty)
            {
                // Nothing of mine to lose. Take the server's copy if it is newer, and
                // leave the row completely alone if it is not - an unconditional
                // overwrite would count a pull that did not happen.
                if (remote.Version > mine.BaseVersion)
                {
                    local.Upsert(new LocalNote(remote.Id, remote.Text, remote.Version, IsDirty: false));
                    pulled++;
                }

                continue;
            }

            if (remote.Version == mine.BaseVersion)
            {
                // The server is still on the version I edited from, so my change applies
                // cleanly. It is now version BaseVersion + 1 and no longer dirty.
                local.Upsert(new LocalNote(mine.Id, mine.Text, mine.BaseVersion + 1, IsDirty: false));
                pushed++;
                continue;
            }

            // Both sides moved. The server wins locally - but the edit that lost is
            // reported rather than dropped. "Last write wins" passes every count in this
            // method and quietly destroys work somebody did on a train with no signal.
            conflicts.Add(new Conflict(mine.Id, mine.Text, remote.Text));
            local.Upsert(new LocalNote(remote.Id, remote.Text, remote.Version, IsDirty: false));
        }

        // Anything dirty that the server has never heard of is new, and goes up.
        foreach (var orphan in local.All().Where(n => n.IsDirty && !seen.Contains(n.Id)))
        {
            local.Upsert(orphan with { BaseVersion = orphan.BaseVersion + 1, IsDirty = false });
            pushed++;
        }

        return new SyncResult(pushed, pulled, conflicts);
    }
}
