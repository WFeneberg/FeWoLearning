using System.Data.Common;

namespace FeWoLearning.Architecture.Exercises.Evolution.Ex074;

// Exercise 074 — ExpandContractMigration (evolution).
// Goal:   Rename a column in a live system, where the old code and the new code are both
//         running and neither can be stopped.
// Drills: the three phases, dual writes, why the middle one exists.
// Passes: Expand   - adds the new column, NULLABLE, and leaves the old one alone. Old
//                    code carries on reading and writing `name` and never notices.
//         THE ONE   - during the middle phase a write must land in BOTH columns. A writer
//                    that only fills the new one leaves every instance still running the
//                    old code reading a null it has no case for.
//         Read     - prefers the new column and falls back to the old, so a row written
//                    before Expand and a row written after both come out right.
//         Backfill - fills the new column for every old row, and is safe to run twice.
//         Contract - drops the old column; reads still work.
//         order    - after Backfill nothing is null, which is the precondition Contract
//                    has and cannot check for itself.
//
// The middle phase is the entire technique, and it is what people skip. Renaming a column
// in one migration is correct for exactly as long as the deployment takes, and a
// deployment is never atomic: for some minutes there are instances running the old code
// and instances running the new one, against one database that can only have one shape.
// Expand-contract makes those minutes uneventful by having the schema support both
// shapes at once, and pays for it with a migration that spans three releases.
//
// Contract is a separate release for a reason: it is the only irreversible step, and it
// must not happen until nothing is reading the old column. That is a fact about deployed
// code, not about the database, and no migration script can check it.
//
// Everything takes a DbConnection and uses "@name" parameters, so the container fact runs
// this same code against real Postgres.
public static class Ex074_ExpandContractMigration
{
    /// <summary>Phase 1: add `display_name`, nullable, keeping `name`.</summary>
    public static void Expand(DbConnection connection) =>
        throw new NotImplementedException(
            "TODO: Ex074 - add a nullable display_name column and leave name untouched");

    /// <summary>Phase 2: write to BOTH columns, because both are being read.</summary>
    public static void WriteDuringMigration(DbConnection connection, string id, string name) =>
        throw new NotImplementedException(
            "TODO: Ex074 - insert the row with the same value in name and display_name");

    /// <summary>Phase 2: prefer the new column, fall back to the old.</summary>
    public static string? ReadDuringMigration(DbConnection connection, string id) =>
        throw new NotImplementedException(
            "TODO: Ex074 - return display_name when it is set, otherwise name");

    /// <summary>Phase 2: fill in the rows written before Expand. Safe to run repeatedly.</summary>
    public static int Backfill(DbConnection connection) =>
        throw new NotImplementedException(
            "TODO: Ex074 - copy name into display_name only where display_name is still null, and report how many rows changed");

    /// <summary>Phase 3, and the only irreversible one: drop `name`.</summary>
    public static void Contract(DbConnection connection) =>
        throw new NotImplementedException("TODO: Ex074 - drop the name column");

    /// <summary>After Contract, only the new column exists.</summary>
    public static string? ReadAfterContract(DbConnection connection, string id) =>
        throw new NotImplementedException("TODO: Ex074 - read display_name");

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
