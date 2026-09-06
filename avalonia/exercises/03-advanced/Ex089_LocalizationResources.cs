using System.Collections.Generic;
using System.Globalization;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 089 - LocalizationResources (advanced).
/// Goal:   Resolve a string for a culture the way resource fallback really works:
///         try the specific culture, then its parent, then the neutral default -
///         and format numbers with the culture rather than with whatever the
///         machine happens to be set to.
/// Drills: CultureInfo and its Parent chain, InvariantCulture, culture-aware
///         formatting, the fallback order every localization stack implements.
/// Passes: dotnet test --filter FullyQualifiedName~Ex089_
///
/// The fallback chain is the content here. "de-AT" is a specific culture whose
/// parent is the neutral "de", whose parent is the invariant culture; a lookup
/// walks that chain and takes the first hit, so a translation supplied only for
/// "de" serves de-DE, de-CH and de-LI alike, and only a genuinely Austrian wording
/// needs a "de-AT" entry of its own. Getting this wrong usually means shipping
/// English to every locale you did not name exactly.
///
/// A warning about the machine this runs on: the ambient culture here is NOT
/// English - it measured de-CH for CurrentCulture and de-DE for CurrentUICulture.
/// Anything culture-dependent must therefore be passed an explicit CultureInfo,
/// never left to the ambient one, or it will pass here and fail on a colleague's
/// machine. That applies to the tests as much as to the code.
public static class Ex089_LocalizationResources
{
    /// <summary>
    /// Given. Do not change. Keyed by culture NAME, with the empty string standing
    /// for the neutral fallback - exactly how a satellite-assembly layout is
    /// organised, minus the assemblies.
    /// </summary>
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

    /// <summary>Given. Do not change. What an unknown key resolves to.</summary>
    public static string Missing(string key) => $"!{key}!";

    /// <summary>
    /// The string for <paramref name="key"/> in <paramref name="culture"/>, taking
    /// the first hit while walking the culture and then its parents, and finally
    /// the neutral entry under the empty-string name.
    ///
    /// Worked examples the test checks:
    ///   de-AT  greeting  -> "Grüß Gott"          (exact hit)
    ///   de-AT  farewell  -> "Auf Wiedersehen"    (falls back to "de")
    ///   de-CH  greeting  -> "Hallo"              (falls back to "de")
    ///   fr-FR  farewell  -> "Goodbye"            ("fr" has no farewell, so neutral)
    ///   en-GB  greeting  -> "Hello"              (no English entries at all)
    ///   any    nothing   -> Missing("nothing")
    ///
    /// Walk the chain by following CultureInfo.Parent until you reach a culture
    /// whose Name is empty - that is the invariant culture and the end of the road.
    /// </summary>
    public static string Lookup(string key, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex089 - walk culture, then its Parent chain, then the neutral " +
            "entry keyed by the empty string, returning the first hit and " +
            "Missing(key) when there is none");

    /// <summary>
    /// A count and its label, formatted for the culture: the number with thousands
    /// separators as that culture writes them, a space, then the "items" string
    /// resolved through Lookup.
    ///
    /// The test compares German, English and invariant output, which differ only in
    /// the separators - so a hard-coded format or the ambient culture fails at
    /// least one of them.
    /// </summary>
    public static string FormatCount(int count, CultureInfo culture) =>
        throw new NotImplementedException(
            "TODO: Ex089 - format count with the \"N0\" numeric format and the given " +
            "culture, then a space, then Lookup(\"items\", culture)");
}
