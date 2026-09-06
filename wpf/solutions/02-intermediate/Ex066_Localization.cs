// Exercise 066 - Localization: satellite resources and switching culture at runtime. REFERENCE SOLUTION.
// Goal:   A real app's strings live in a .resx plus one satellite .resx per language, read
//         through a ResourceManager that follows Thread.CurrentUICulture - never a parameter you
//         pass to it directly. This row's own two content libraries carry real satellite
//         resources (Ex066_Strings.resx, the neutral/English default, and Ex066_Strings.de.resx,
//         the German satellite) - this track's first non-.cs content files, after 65 exercises
//         that each found a way to avoid one (row 038 chose an in-memory configuration source
//         over a JSON file; row 058 chose code over XamlReader.Parse). Localization genuinely
//         IS ResourceManager plus satellites, though, so this row pays that cost rather than
//         substitute something that would teach nothing.
// Drills: ResourceManager (via the ready-to-use Ex066_Strings wrapper below - not the part you
//         write), and switching Thread.CurrentUICulture at runtime to control which satellite a
//         lookup resolves - restoring it afterward, even if the read itself throws.

using System.Globalization;
using System.Resources;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ready to use - wraps the manifest resource both Ex066_Strings.resx (neutral) and
/// Ex066_Strings.de.resx (German satellite) compile into. Not the subject of this row: what
/// matters is that both properties below read via <see cref="CultureInfo.CurrentUICulture"/>,
/// never a culture you could pass in directly - that is the whole reason GreetIn below has
/// anything to switch.
/// </summary>
public static class Ex066_Strings
{
    private static readonly ResourceManager Resources =
        new("FeWoLearning.Wpf.Exercises.Intermediate.Ex066_Strings", typeof(Ex066_Strings).Assembly);

    public static string Greeting => Resources.GetString("Greeting", CultureInfo.CurrentUICulture) ?? string.Empty;

    public static string Farewell => Resources.GetString("Farewell", CultureInfo.CurrentUICulture) ?? string.Empty;
}

public static class Ex066_Localization
{
    /// <summary>
    /// Reads <see cref="Ex066_Strings.Greeting"/> as it would resolve under <paramref name="culture"/>
    /// - "Hallo" for German, "Hello" for English or any culture with no satellite of its own (the
    /// fallback chain lands on the neutral resource) - without leaking that culture change into
    /// whatever runs after this call.
    /// </summary>
    public static string GreetIn(CultureInfo culture)
    {
        var previous = Thread.CurrentThread.CurrentUICulture;
        Thread.CurrentThread.CurrentUICulture = culture;
        try
        {
            return Ex066_Strings.Greeting;
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previous;
        }
    }
}
