using System.Data.Common;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex038;

public sealed record Document(string Id, string Text, int Version);

// Exercise 038 — OptimisticConcurrency (reference solution).
// Goal:   Let two people edit the same row without locking anything, and make sure the
//         second one is TOLD rather than silently overwriting the first.
// Drills: version column, conflict detection, lost-update prevention.
// Passes: current version - the update succeeds and the version moves on by one.
//         stale version   - the update FAILS, returns false, and leaves the row exactly
//                           as the winner left it.
//         THE ONE          - a stale writer whose new text happens to EQUAL what is
//                           already stored still fails.
//         a second attempt from the reloaded version succeeds.
//
// The equal-text clause separates a version column from a content comparison. Comparing
// what the row holds against what the writer expects to find - "nothing changed, so it
// is safe" - passes every other assertion here, and gets it wrong in the one case that
// matters: A wrote "approved", B (who never saw that) also writes "approved" from a
// stale read, the comparison sees no difference and lets it through. The text is
// identical; the DECISION was not, and B just silently overwrote a state it never
// observed. The version says so, the content cannot.
//
// Everything takes a Func<DbConnection> and "@name" parameters, so the container fact
// runs this same code against real Postgres.
public sealed class VersionedDocumentStore(Func<DbConnection> openConnection)
{
    public Document? Read(string id)
    {
        using var connection = openConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, text, version FROM documents WHERE id = @id";
        AddParameter(command, "@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read()
            ? new Document(reader.GetString(0), reader.GetString(1), Convert.ToInt32(reader.GetValue(2)))
            : null;
    }

    public void Insert(Document document)
    {
        using var connection = openConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO documents (id, text, version) VALUES (@id, @text, @version)";
        AddParameter(command, "@id", document.Id);
        AddParameter(command, "@text", document.Text);
        AddParameter(command, "@version", document.Version);
        command.ExecuteNonQuery();
    }

    public bool TryUpdate(Document expected, string newText)
    {
        using var connection = openConnection();
        using var command = connection.CreateCommand();

        // The version is in the WHERE clause, and that is the entire mechanism: the
        // database decides, atomically, whether this writer was looking at the row it is
        // about to overwrite. Reading the version first and comparing it in application
        // code reopens the very window the check exists to close.
        command.CommandText = """
            UPDATE documents
               SET text = @text, version = version + 1
             WHERE id = @id AND version = @version
            """;
        AddParameter(command, "@text", newText);
        AddParameter(command, "@id", expected.Id);
        AddParameter(command, "@version", expected.Version);

        // Zero rows affected means somebody else got there first. Not an error - a
        // conflict, and the caller decides what to do about it.
        return command.ExecuteNonQuery() == 1;
    }

    public static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
