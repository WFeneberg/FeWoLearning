using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Gallery;

namespace FeWoLearning.Avalonia.Tests;

public class GallerySmokeTests
{
    [Fact]
    public void Every_Registered_Entry_Has_A_Three_Digit_Id_And_A_Title()
    {
        Assert.NotEmpty(GalleryCatalog.Entries);

        foreach (var entry in GalleryCatalog.Entries)
        {
            Assert.Matches(@"^\d{3}$", entry.Id);
            Assert.False(string.IsNullOrWhiteSpace(entry.Title));
        }
    }

    [Fact]
    public void Ids_Are_Unique_And_Ascending()
    {
        var ids = GalleryCatalog.Entries.Select(e => e.Id).ToList();

        Assert.Equal(ids.Distinct().Count(), ids.Count);
        Assert.Equal(ids.OrderBy(id => id).ToList(), ids);
    }

    // Constructing a page reaches straight into the exercise's constructor, so this
    // asserts the red/green mechanism as much as the gallery: in exercises mode every
    // page must surface the stub's NotImplementedException, and in solutions mode
    // every page must build. A page that succeeds in exercises mode means its stub
    // forgot to throw.
    [AvaloniaFact]
    public void Every_Page_Builds_In_Solutions_Mode_And_Throws_In_Exercises_Mode()
    {
        foreach (var entry in GalleryCatalog.Entries)
        {
            if (ViewHarness.SolutionsMode)
            {
                Assert.NotNull(entry.Create());
                continue;
            }

            var error = Record.Exception(() => entry.Create());
            Assert.NotNull(error);
            Assert.Contains($"TODO: Ex{entry.Id}", Flatten(error!));
        }
    }

    private static string Flatten(Exception ex) =>
        ex.InnerException is null ? ex.Message : $"{ex.Message} {Flatten(ex.InnerException)}";
}
