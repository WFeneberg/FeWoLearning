using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex007_ScreenDisplayNameTests : CaliburnCoreContext
{
    private static List<string?> RecordDisplayNameChanges(Ex007_ScreenDisplayName vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(vm.DisplayName)) names.Add(vm.DisplayName);
        };
        return names;
    }

    [Fact]
    public void Fresh_Screen_Defaults_DisplayName_To_Its_Own_Type_Full_Name_Then_Rename_Overwrites_It()
    {
        var vm = new Ex007_ScreenDisplayName();

        // Caliburn sets this before any of your code runs - it is not null or empty.
        Assert.Equal(typeof(Ex007_ScreenDisplayName).FullName, vm.DisplayName);

        vm.Rename("Report.docx");

        Assert.Equal("Report.docx", vm.DisplayName);
    }

    [Fact]
    public void Rename_Raises_Exactly_One_PropertyChanged_For_DisplayName()
    {
        var vm = new Ex007_ScreenDisplayName();
        var changes = RecordDisplayNameChanges(vm);

        vm.Rename("Report.docx");

        Assert.Equal(new[] { "Report.docx" }, changes);
    }

    [Fact]
    public void Renaming_To_The_Same_Name_Still_Announces_DisplayName()
    {
        var vm = new Ex007_ScreenDisplayName();
        vm.Rename("Report.docx");
        var changes = RecordDisplayNameChanges(vm);

        // Same string as before. Unlike PropertyChangedBase.Set (ex002), Screen.DisplayName
        // has no equality check - a naive "skip if unchanged" guard would wrongly swallow
        // this one.
        vm.Rename("Report.docx");

        Assert.Equal(new[] { "Report.docx" }, changes);
    }

    [Fact]
    public void MarkDirty_Appends_A_Marker_Without_Touching_The_Document_Name()
    {
        var vm = new Ex007_ScreenDisplayName();
        vm.Rename("Report.docx");

        vm.MarkDirty();

        Assert.Equal("Report.docx *", vm.DisplayName);
    }

    [Fact]
    public void Save_Clears_The_Dirty_Marker()
    {
        var vm = new Ex007_ScreenDisplayName();
        vm.Rename("Report.docx");
        vm.MarkDirty();

        vm.Save();

        Assert.Equal("Report.docx", vm.DisplayName);
    }
}
