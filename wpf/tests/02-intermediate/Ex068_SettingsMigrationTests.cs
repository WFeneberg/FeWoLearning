using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex068_SettingsMigrationTests : WpfTestContext
{
    [WpfFact]
    public void V1_Input_With_No_SchemaVersion_Key_Migrates_UserName_To_DisplayName()
    {
        var raw = new Dictionary<string, object?> { ["UserName"] = "Carol" };

        var result = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal(2, result.SchemaVersion);
        Assert.Equal("Carol", result.DisplayName);
        Assert.Equal("Light", result.Theme);
    }

    [WpfFact]
    public void A_Different_V1_UserName_Produces_A_Different_DisplayName()
    {
        // Vary the input across call sites - rejects a hard-coded "Carol" return value.
        var raw = new Dictionary<string, object?> { ["UserName"] = "Dana" };

        var result = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal("Dana", result.DisplayName);
    }

    [WpfFact]
    public void Explicit_SchemaVersion_1_Is_Treated_The_Same_As_A_Missing_Key()
    {
        var raw = new Dictionary<string, object?> { ["SchemaVersion"] = 1, ["UserName"] = "Erin" };

        var result = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal(2, result.SchemaVersion);
        Assert.Equal("Erin", result.DisplayName);
    }

    [WpfFact]
    public void V1_Input_With_An_Explicit_Theme_Preserves_It_Rather_Than_Overwriting_With_The_Default()
    {
        var raw = new Dictionary<string, object?> { ["UserName"] = "Carol", ["Theme"] = "Dark" };

        var result = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal("Dark", result.Theme);
    }

    [WpfFact]
    public void Already_Current_Data_Is_Read_Directly_Not_By_Checking_For_A_Legacy_Field()
    {
        // Already migrated: SchemaVersion says 2, and DisplayName was since edited by the user
        // through the current-version UI - but a stale legacy UserName key is still lingering
        // (never cleaned up by a prior migration run). A correct migrator trusts the stamp and
        // leaves DisplayName alone; a migrator that decides "is this V1?" by checking whether
        // UserName is present instead re-migrates and clobbers the real value with the stale one
        // - the same bug shape as re-running the V1 transform a second time on already-current
        // data.
        var raw = new Dictionary<string, object?>
        {
            ["SchemaVersion"] = 2,
            ["DisplayName"] = "Alice",
            ["UserName"] = "Bob",
            ["Theme"] = "Dark",
        };

        var result = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal(2, result.SchemaVersion);
        Assert.Equal("Alice", result.DisplayName);
        Assert.Equal("Dark", result.Theme);
    }

    [WpfFact]
    public void Migrating_The_Same_Already_Current_Input_Twice_Produces_The_Identical_Result()
    {
        var raw = new Dictionary<string, object?>
        {
            ["SchemaVersion"] = 2,
            ["DisplayName"] = "Frank",
            ["Theme"] = "Dark",
        };

        var first = Ex068_SettingsMigrator.Migrate(raw);
        var second = Ex068_SettingsMigrator.Migrate(raw);

        Assert.Equal(first.SchemaVersion, second.SchemaVersion);
        Assert.Equal(first.DisplayName, second.DisplayName);
        Assert.Equal(first.Theme, second.Theme);
    }
}
