using System.Globalization;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex066_LocalizationTests : WpfTestContext
{
    // Every test below pins Thread.CurrentUICulture to the OPPOSITE language from what it asks
    // GreetIn for, before calling it - not to some machine-dependent "whatever it already is".
    // That is what actually proves the switch is real: a mutant that ignores `culture` and just
    // reads Ex066_Strings.Greeting against ambient state would echo back the WRONG language, not
    // coincidentally the right one, regardless of this machine's own OS locale.

    [WpfFact]
    public void GreetIn_English_Reads_The_Neutral_Resource_Even_When_The_Ambient_Culture_Is_German()
    {
        var previous = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

            var result = Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("en-US"));

            Assert.Equal("Hello", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previous;
        }
    }

    [WpfFact]
    public void GreetIn_German_Reads_The_German_Satellite_Even_When_The_Ambient_Culture_Is_English()
    {
        var previous = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var result = Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("de-DE"));

            Assert.Equal("Hallo", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previous;
        }
    }

    [WpfFact]
    public void GreetIn_A_Culture_With_No_Satellite_Falls_Back_To_The_Neutral_Resource()
    {
        // Vary inputs across call sites, and reject a mutant hard-coding "de-DE -> Hallo, else
        // Hello": fr-FR has no satellite of its own, so the fallback chain lands on the same
        // neutral resource en-US reads - proving this is real ResourceManager fallback, not a
        // two-way lookup table.
        var previous = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

            var result = Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("fr-FR"));

            Assert.Equal("Hello", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previous;
        }
    }

    [WpfFact]
    public void GreetIn_Restores_CurrentUICulture_Afterward_Across_Several_Calls()
    {
        var previous = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("de-DE"));
            Assert.Equal(previous, Thread.CurrentThread.CurrentUICulture);

            Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("fr-FR"));
            Assert.Equal(previous, Thread.CurrentThread.CurrentUICulture);
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = previous;
        }
    }

    [WpfFact]
    public void GreetIn_Follows_CurrentUICulture_Not_CurrentCulture()
    {
        // Rejects the mutant that sets the wrong Thread property: CurrentCulture is forced to
        // German here and CurrentUICulture to English - a ResourceManager lookup only ever
        // follows CurrentUICulture, so an implementation that (mistakenly) assigns `culture` to
        // CurrentCulture instead never actually changes the ambient CurrentUICulture the lookup
        // reads, and would return "Hello" here instead of "Hallo".
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        var previousUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var result = Ex066_Localization.GreetIn(CultureInfo.GetCultureInfo("de-DE"));

            Assert.Equal("Hallo", result);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
            Thread.CurrentThread.CurrentUICulture = previousUICulture;
        }
    }
}
