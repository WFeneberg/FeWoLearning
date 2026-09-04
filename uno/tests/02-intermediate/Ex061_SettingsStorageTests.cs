using FeWoLearning.Uno.Exercises.Intermediate;
using Windows.Storage;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex061_SettingsStorageTests : UnoTestContext
{
    private static Ex061_SettingsStorage Settings() => new(ApplicationData.Current.LocalSettings);

    /// <summary>
    /// A key nothing else uses. Uno does not implement
    /// ApplicationDataContainer.CreateContainer, so a nested container per test is not
    /// available and unique keys are how the tests stay independent - of each other, and of
    /// whatever the real app has stored.
    /// </summary>
    private static string Key() => "Ex061_" + Guid.NewGuid().ToString("N");

    [Fact]
    public void An_Unwritten_Key_Falls_Back()
    {
        var settings = Settings();
        var key = Key();

        Assert.Equal(7, settings.GetInt(key, fallback: 7));
        Assert.False(settings.Has(key));
    }

    [Fact]
    public void A_Written_Value_Comes_Back()
    {
        var settings = Settings();
        var key = Key();

        settings.SetInt(key, 3);

        Assert.Equal(3, settings.GetInt(key, fallback: 7));
        Assert.True(settings.Has(key));
    }

    [Fact]
    public void Writing_The_Fallback_Value_Still_Counts_As_Written()
    {
        var settings = Settings();
        var key = Key();

        settings.SetInt(key, 7);

        // "Absent" and "happens to equal the default" are different states, and only one of
        // them should be overwritten by a future default change.
        Assert.True(settings.Has(key));
    }

    [Fact]
    public void A_Value_Can_Be_Overwritten()
    {
        var settings = Settings();
        var key = Key();

        settings.SetInt(key, 3);
        settings.SetInt(key, 5);

        Assert.Equal(5, settings.GetInt(key, fallback: 7));
    }

    [Fact]
    public void Forgetting_Restores_The_Fallback()
    {
        var settings = Settings();
        var key = Key();
        settings.SetInt(key, 3);

        settings.Forget(key);

        Assert.Equal(7, settings.GetInt(key, fallback: 7));
        Assert.False(settings.Has(key));
    }

    [Fact]
    public void Forgetting_An_Absent_Key_Is_Harmless()
    {
        var settings = Settings();
        var key = Key();

        settings.Forget(key);

        Assert.False(settings.Has(key));
    }

    [Fact]
    public void Keys_Are_Independent()
    {
        var settings = Settings();
        var zoom = Key();
        var volume = Key();

        settings.SetInt(zoom, 3);
        settings.SetInt(volume, 9);
        settings.Forget(zoom);

        Assert.Equal(9, settings.GetInt(volume, fallback: 0));
    }

    [Fact]
    public void A_Value_Of_The_Wrong_Type_Falls_Back()
    {
        var key = Key();
        ApplicationData.Current.LocalSettings.Values[key] = "three";
        var settings = Settings();

        // The bag holds object, so a previous version of the app really can have written a
        // string here. A cast would throw on the user's machine, at startup.
        Assert.Equal(7, settings.GetInt(key, fallback: 7));
    }

    [Fact]
    public void The_Values_Survive_A_New_Wrapper()
    {
        var key = Key();

        Settings().SetInt(key, 3);

        // The store is the container, not the wrapper - which is what makes a setting
        // survive a restart in a real app.
        Assert.Equal(3, Settings().GetInt(key, fallback: 7));
    }
}
