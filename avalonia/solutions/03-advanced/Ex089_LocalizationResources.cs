using System.Collections.Generic;
using System.Globalization;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex089_
public static class Ex089_LocalizationResources
{
    /// <summary>Given. Do not change.</summary>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Catalog { get; } =
        new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            [""] = new Dictionary<string, string>
            {
                ["greeting"] = "Hello",
                ["farewell"] = "Goodbye",
                ["items"] = "items",
            },
            ["de"] = new Dictionary<string, string>
            {
                ["greeting"] = "Hallo",
                ["farewell"] = "Auf Wiedersehen",
            },
            ["de-AT"] = new Dictionary<string, string>
            {
                ["greeting"] = "Grüß Gott",
            },
            ["fr"] = new Dictionary<string, string>
            {
                ["greeting"] = "Bonjour",
            },
        };

    /// <summary>Given. Do not change.</summary>
    public static string Missing(string key) => $"!{key}!";

    public static string Lookup(string key, CultureInfo culture)
    {
        // Walk the chain, taking the first hit: de-AT, then de, then invariant -
        // whose Name is the empty string, which is also the neutral entry's key, so
        // the loop ends by trying exactly the right thing.
        for (var current = culture; ; current = current.Parent)
        {
            if (Catalog.TryGetValue(current.Name, out var entries) &&
                entries.TryGetValue(key, out var value))
            {
                return value;
            }

            if (current.Name.Length == 0)
            {
                return Missing(key);
            }
        }
    }

    public static string FormatCount(int count, CultureInfo culture) =>
        $"{count.ToString("N0", culture)} {Lookup("items", culture)}";
}
