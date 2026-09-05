using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex023;

public sealed record Settings(int Version, string Theme, int FontSize, string Language);

// Exercise 023 — SettingsStatePersistence (desktop).
// Goal:   Load a settings file written by an older build of the application, by
//         MIGRATING it step by step rather than by giving up and using defaults.
// Drills: versioned settings, migration chain, forward compatibility.
// Passes: v3 file - loads unchanged.
//         v1 file - becomes v3: Language gains the v2 default "en", and Theme is
//                   normalised by the v3 step ("dark" -> "Dark").
//         v2 file - only the v3 step runs, so a Language the user actually chose - say
//                   "de" - SURVIVES instead of reverting to "en".
//         v4 file - NotSupportedException naming the version; a file from the future is
//                   not something to guess at.
//         Save    - always stamps CurrentVersion, and Save/Load round-trips.
//
// The v2 case is the one that separates a migration chain from the shortcut. "If the
// version is old, load defaults" passes the v1 case perfectly, and silently resets the
// preferences of every user who upgraded from v2 - a bug that looks like data loss to
// them and like nothing at all in the logs.
//
// Schema history:
//   v1: { "version": 1, "theme": "dark", "fontSize": 12 }
//   v2: adds "language", whose default for an upgraded file is "en"
//   v3: normalises "theme" to title case ("dark" -> "Dark", "light" -> "Light")
public static class Ex023_SettingsStatePersistence
{
    public const int CurrentVersion = 3;

    /// <summary>Read a settings document of any supported version and return it as v3.</summary>
    public static Settings Load(string json) =>
        throw new NotImplementedException(
            "TODO: Ex023 - read the version, run each migration step in turn up to CurrentVersion, and reject anything newer");

    /// <summary>Write settings, always stamped with <see cref="CurrentVersion"/>.</summary>
    public static string Save(Settings settings) =>
        throw new NotImplementedException(
            "TODO: Ex023 - serialise the settings with version set to CurrentVersion");
}
