using System.Data.Common;

namespace FeWoLearning.Architecture.Exercises.Evolution.Ex074;

// Exercise 074 — ExpandContractMigration (reference solution).
public static class Ex074_ExpandContractMigration
{
    public static void Expand(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        // NULLABLE, and `name` stays. A NOT NULL column with no default fails outright on
        // a table with rows; one with a default rewrites the whole table and locks it.
        // Nullable-then-backfill is what makes this phase boring, which is the point.
        command.CommandText = "ALTER TABLE users ADD COLUMN display_name TEXT";
        command.ExecuteNonQuery();
    }

    public static void WriteDuringMigration(DbConnection connection, string id, string name)
    {
        using var command = connection.CreateCommand();
        // BOTH columns. A writer that fills only the new one leaves every instance still
        // running the old code reading a null it has no case for - and those instances
        // exist, for the length of the deployment, by definition.
        command.CommandText = "INSERT INTO users (id, name, display_name) VALUES (@id, @name, @name)";
        AddParameter(command, "@id", id);
        AddParameter(command, "@name", name);
        command.ExecuteNonQuery();
    }

    public static string? ReadDuringMigration(DbConnection connection, string id)
    {
        using var command = connection.CreateCommand();
        // New first, old as the fallback: a row written before Expand has no display_name
        // yet, and the new code must not treat that as a missing user.
        command.CommandText = "SELECT COALESCE(display_name, name) FROM users WHERE id = @id";
        AddParameter(command, "@id", id);
        return command.ExecuteScalar() as string;
    }

    public static int Backfill(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        // "WHERE display_name IS NULL" makes it re-runnable and, more importantly, keeps
        // it from overwriting a value the new code has already written - a backfill that
        // copies unconditionally undoes every edit made since Expand.
        command.CommandText = "UPDATE users SET display_name = name WHERE display_name IS NULL";
        return command.ExecuteNonQuery();
    }

    public static void Contract(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        // The only irreversible step, and a separate release for that reason: it must not
        // run until nothing is reading `name`, which is a fact about deployed code that no
        // migration script can check.
        command.CommandText = "ALTER TABLE users DROP COLUMN name";
        command.ExecuteNonQuery();
    }

    public static string? ReadAfterContract(DbConnection connection, string id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT display_name FROM users WHERE id = @id";
        AddParameter(command, "@id", id);
        return command.ExecuteScalar() as string;
    }

    /// <summary>Test helper: how many rows still have a null display_name.</summary>
    public static int RowsMissingDisplayName(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM users WHERE display_name IS NULL";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>Shared helper: adds a parameter without naming a provider.</summary>
    public static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
