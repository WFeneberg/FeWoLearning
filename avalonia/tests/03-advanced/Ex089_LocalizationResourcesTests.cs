using System.Globalization;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Advanced;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex089_LocalizationResourcesTests
{
    // Every culture is passed explicitly and never taken from the ambient one,
    // which on this machine is de-CH / de-DE rather than anything English - so a
    // test that relied on the default would be measuring the machine.
    private static string Lookup(string key, string culture) =>
        Ex089_LocalizationResources.Lookup(key, new CultureInfo(culture));

    [AvaloniaTheory]
    // An exact hit on the most specific culture.
    [InlineData("greeting", "de-AT", "Grüß Gott")]
    // The specific culture has no entry for this key, so its parent answers.
    [InlineData("farewell", "de-AT", "Auf Wiedersehen")]
    // No de-CH entries at all, so the whole lookup lands on the neutral German.
    [InlineData("greeting", "de-CH", "Hallo")]
    // "fr" exists but has no farewell, so this reaches the invariant fallback -
    // the case an implementation that stops at the first matching CULTURE, rather
    // than the first matching KEY, gets wrong.
    [InlineData("farewell", "fr-FR", "Goodbye")]
    [InlineData("greeting", "fr-FR", "Bonjour")]
    // Nothing English in the catalog, so everything falls through.
    [InlineData("greeting", "en-GB", "Hello")]
    [InlineData("farewell", "en-GB", "Goodbye")]
    public void The_Fallback_Chain_Takes_The_First_Hit(string key, string culture, string expected)
    {
        Assert.Equal(expected, Lookup(key, culture));
    }

    [AvaloniaFact]
    public void A_Neutral_Culture_Resolves_Against_Its_Own_Entries()
    {
        Assert.Equal("Hallo", Lookup("greeting", "de"));
        Assert.Equal("Auf Wiedersehen", Lookup("farewell", "de"));
    }

    [AvaloniaFact]
    public void The_Invariant_Culture_Gets_The_Neutral_Entries()
    {
        Assert.Equal("Hello", Ex089_LocalizationResources.Lookup("greeting", CultureInfo.InvariantCulture));
    }

    // The chain has to terminate rather than loop: the invariant culture is its
    // own parent, so a walk written as "keep going until the parent changes" spins
    // forever.
    [AvaloniaFact]
    public void An_Unknown_Key_Falls_All_The_Way_Through()
    {
        Assert.Equal(Ex089_LocalizationResources.Missing("nothing"), Lookup("nothing", "de-AT"));
        Assert.Equal(Ex089_LocalizationResources.Missing("nothing"), Lookup("nothing", "en-GB"));
        Assert.Equal(
            Ex089_LocalizationResources.Missing("nothing"),
            Ex089_LocalizationResources.Lookup("nothing", CultureInfo.InvariantCulture));
    }

    // Formatting is the other half, and the separators are what a hard-coded
    // format string or the ambient culture gets wrong.
    [AvaloniaTheory]
    [InlineData("de-DE", "1.234.567 items")]
    [InlineData("en-US", "1,234,567 items")]
    [InlineData("fr-FR", "1 234 567 items")]
    public void A_Count_Is_Formatted_For_Its_Culture(string culture, string expected)
    {
        Assert.Equal(expected, Ex089_LocalizationResources.FormatCount(1234567, new CultureInfo(culture)));
    }

    [AvaloniaFact]
    public void A_Small_Count_Needs_No_Separators_In_Any_Culture()
    {
        Assert.Equal("42 items", Ex089_LocalizationResources.FormatCount(42, new CultureInfo("de-DE")));
        Assert.Equal("42 items", Ex089_LocalizationResources.FormatCount(42, new CultureInfo("en-US")));
    }

    // The label comes through the same fallback chain as anything else, so a
    // German count still says "items" - the catalog has no German word for it, and
    // inventing one is not the exercise.
    [AvaloniaFact]
    public void The_Label_Is_Resolved_Through_The_Same_Fallback()
    {
        Assert.EndsWith(
            Ex089_LocalizationResources.Lookup("items", new CultureInfo("de-AT")),
            Ex089_LocalizationResources.FormatCount(5, new CultureInfo("de-AT")));
    }
}
