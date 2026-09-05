using FeWoLearning.Architecture.Exercises.Desktop.Ex023;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex023_SettingsStatePersistenceTests
{
    private const string V1 = """{"version":1,"theme":"dark","fontSize":14}""";
    private const string V2 = """{"version":2,"theme":"dark","fontSize":14,"language":"de"}""";
    private const string V3 = """{"version":3,"theme":"Dark","fontSize":14,"language":"de"}""";

    [Fact]
    public void A_Current_File_Loads_Unchanged()
    {
        var settings = Ex023_SettingsStatePersistence.Load(V3);

        Assert.Equal(3, settings.Version);
        Assert.Equal("Dark", settings.Theme);
        Assert.Equal(14, settings.FontSize);
        Assert.Equal("de", settings.Language);
    }

    [Fact]
    public void A_V1_File_Is_Migrated_All_The_Way_Forward()
    {
        var settings = Ex023_SettingsStatePersistence.Load(V1);

        Assert.Equal(Ex023_SettingsStatePersistence.CurrentVersion, settings.Version);
        Assert.Equal("en", settings.Language);   // the v2 step supplied it
        Assert.Equal("Dark", settings.Theme);    // the v3 step normalised it
        Assert.Equal(14, settings.FontSize);     // and nothing invented a default here
    }

    [Fact]
    public void Adversarial_A_V2_File_Keeps_The_Language_The_User_Chose()
    {
        // The fact that separates a migration chain from the shortcut. "If the version
        // is old, load defaults" passes the v1 case perfectly, and silently resets the
        // preferences of everyone who upgraded from v2 - data loss to them, nothing at
        // all in the logs.
        var settings = Ex023_SettingsStatePersistence.Load(V2);

        Assert.Equal("de", settings.Language);
        Assert.Equal("Dark", settings.Theme);
    }

    [Fact]
    public void A_File_From_The_Future_Is_Refused_By_Version()
    {
        var fromTheFuture = """{"version":4,"theme":"Dark","fontSize":14,"language":"de","spacing":2}""";

        var failure = Assert.Throws<NotSupportedException>(
            () => Ex023_SettingsStatePersistence.Load(fromTheFuture));

        Assert.Contains("4", failure.Message);
    }

    [Fact]
    public void Save_Always_Stamps_The_Current_Version()
    {
        // Writing back whatever version came in is how a file gets migrated on load and
        // then saved as v1 again, so the migration runs afresh on every start.
        var json = Ex023_SettingsStatePersistence.Save(new Settings(1, "Dark", 14, "de"));

        Assert.Contains("\"version\":3", json);
    }

    [Fact]
    public void Save_And_Load_Round_Trip()
    {
        var original = new Settings(3, "Light", 11, "fr");

        var restored = Ex023_SettingsStatePersistence.Load(Ex023_SettingsStatePersistence.Save(original));

        Assert.Equal(original, restored);
    }
}
