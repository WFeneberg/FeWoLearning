using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex023;

public sealed record Settings(int Version, string Theme, int FontSize, string Language);

// Exercise 023 — SettingsStatePersistence (reference solution).
public static class Ex023_SettingsStatePersistence
{
    public const int CurrentVersion = 3;

    public static Settings Load(string json)
    {
        var document = JsonNode.Parse(json)?.AsObject()
            ?? throw new ArgumentException("Not a settings document.", nameof(json));

        var version = document["version"]?.GetValue<int>() ?? 1;

        if (version > CurrentVersion)
            throw new NotSupportedException(
                $"Settings version {version} is newer than this build understands ({CurrentVersion}).");

        // A chain of steps, each one moving the document forward exactly one version.
        // The shortcut - "if it is old, load defaults" - passes a v1 file perfectly and
        // silently resets the preferences of everyone who upgraded from v2.
        if (version < 2)
        {
            document["language"] = "en";
            version = 2;
        }

        if (version < 3)
        {
            var theme = document["theme"]?.GetValue<string>() ?? "light";
            document["theme"] = Normalise(theme);
            version = 3;
        }

        return new Settings(
            CurrentVersion,
            document["theme"]?.GetValue<string>() ?? "Light",
            document["fontSize"]?.GetValue<int>() ?? 12,
            document["language"]?.GetValue<string>() ?? "en");
    }

    public static string Save(Settings settings) =>
        JsonSerializer.Serialize(new
        {
            version = CurrentVersion,
            theme = settings.Theme,
            fontSize = settings.FontSize,
            language = settings.Language,
        });

    private static string Normalise(string theme) =>
        theme.Length == 0 ? theme : char.ToUpperInvariant(theme[0]) + theme[1..];
}
