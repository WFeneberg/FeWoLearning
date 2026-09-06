using Microsoft.Extensions.Configuration;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex057;

// Exercise 057 — ConfigurationLayering (cross-cutting).
// Goal:   Stack configuration sources so the more specific one wins, and know exactly
//         what "wins" means when a value is present but empty.
// Drills: provider precedence, layering, reload.
// Passes: base only        - a key only the defaults define is readable.
//         precedence       - environment beats defaults; secrets beat environment. Last
//                            registered wins, and that is the ONLY rule.
//         absence          - a key a later layer does not mention leaves the earlier
//                            layer's value alone.
//         THE ONE           - a key a later layer sets to the EMPTY STRING does override.
//                            Present-and-empty is a value; absent is not.
//         reload           - changing a source and reloading is visible.
//
// The empty-string case is where this stops being obvious, and both readings are
// defensible until somebody picks one. "An empty value means unset, fall through" is what
// people assume, and it is not what Microsoft.Extensions.Configuration does - which
// matters the day an operator sets PROXY_URL to nothing precisely in order to disable the
// proxy, and gets the default back instead. It has to be decided, written down, and
// asserted; leaving it to whichever provider happens to be last is the actual bug.
public static class Ex057_ConfigurationLayering
{
    /// <summary>
    /// Build a configuration from the three layers, weakest first: defaults, then
    /// environment, then secrets.
    /// </summary>
    public static IConfigurationRoot Build(
        IReadOnlyDictionary<string, string?> defaults,
        IReadOnlyDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?> secrets) =>
        throw new NotImplementedException(
            "TODO: Ex057 - add the three in-memory sources in precedence order and build the root");

    /// <summary>
    /// Which layer a key's effective value came from: "secrets", "environment",
    /// "defaults", or null when nothing defines it. A key present in a layer counts,
    /// even if its value is empty.
    /// </summary>
    public static string? SourceOf(
        string key,
        IReadOnlyDictionary<string, string?> defaults,
        IReadOnlyDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?> secrets) =>
        throw new NotImplementedException(
            "TODO: Ex057 - report the strongest layer that CONTAINS the key, not the strongest one with a non-empty value");
}
