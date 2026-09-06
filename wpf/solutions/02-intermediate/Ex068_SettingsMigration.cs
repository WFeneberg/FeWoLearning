// Exercise 068 - A versioned settings store upgrading an old shape. REFERENCE SOLUTION.
// Goal:   Load a settings blob that might be years old - shaped however the FIRST version of the
//         app wrote it - and normalize it into today's shape exactly once, trusting an explicit
//         version stamp to decide whether migration is even needed, never the mere presence of
//         some field that happens to distinguish old from new. Guessing from field presence looks
//         equivalent until an old field lingers in an already-current record (a prior migration
//         that never cleaned up after itself, say) - then it silently re-migrates data that was
//         already correct.
// Drills: reading an explicit "SchemaVersion" stamp (treating a MISSING key as version 1, never
//         inferring "this is old" from some other field's presence), renaming a legacy field into
//         its current-shape name, defaulting a field that did not exist at all in the old shape,
//         and writing the stamp forward to the current version - idempotently, so migrating
//         already-current data a second time changes nothing.

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>Ready to use - the current, in-memory shape Migrate below produces.</summary>
public sealed class Ex068_Settings
{
    public int SchemaVersion { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Theme { get; init; } = "Light";
}

public static class Ex068_SettingsMigrator
{
    public const int CurrentSchemaVersion = 2;

    /// <summary>
    /// Reads <paramref name="raw"/> - a settings blob shaped either like today's version
    /// (<see cref="CurrentSchemaVersion"/>) or like version 1, the shape before "UserName" was
    /// renamed to "DisplayName" and "Theme" existed at all - and returns it as an
    /// <see cref="Ex068_Settings"/> already on the current schema.
    /// </summary>
    public static Ex068_Settings Migrate(IReadOnlyDictionary<string, object?> raw)
    {
        var schemaVersion = raw.TryGetValue("SchemaVersion", out var stamp) && stamp is int v ? v : 1;
        var theme = raw.TryGetValue("Theme", out var themeValue) && themeValue is string t ? t : "Light";

        if (schemaVersion >= CurrentSchemaVersion)
        {
            var displayName = raw.TryGetValue("DisplayName", out var name) && name is string n ? n : string.Empty;

            return new Ex068_Settings
            {
                SchemaVersion = CurrentSchemaVersion,
                DisplayName = displayName,
                Theme = theme,
            };
        }

        var legacyUserName = raw.TryGetValue("UserName", out var userName) && userName is string u ? u : string.Empty;

        return new Ex068_Settings
        {
            SchemaVersion = CurrentSchemaVersion,
            DisplayName = legacyUserName,
            Theme = theme,
        };
    }
}
