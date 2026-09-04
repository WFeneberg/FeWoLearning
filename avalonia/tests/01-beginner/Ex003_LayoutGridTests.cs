using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex003_LayoutGridTests
{
    // 200 x 200: column 0 is a fixed 80, column 1 takes the remaining 120.
    // Row 0 is Auto and the header cells are 24 tall, so row 1 gets 176.
    private static Ex003_LayoutGrid Show() =>
        ViewHarness.Show(new Ex003_LayoutGrid(), 200, 200);

    [AvaloniaFact]
    public void Fixed_Column_Is_Eighty_And_The_Star_Column_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 80, 24), view.FindControl<Border>("HeaderLeft")!.Bounds);
        Assert.Equal(new Rect(80, 0, 120, 24), view.FindControl<Border>("HeaderRight")!.Bounds);
    }

    [AvaloniaFact]
    public void Auto_Row_Takes_Its_Height_From_The_Header_And_The_Star_Row_Takes_The_Rest()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 24, 80, 176), view.FindControl<Border>("BodyLeft")!.Bounds);
        Assert.Equal(new Rect(80, 24, 120, 176), view.FindControl<Border>("BodyRight")!.Bounds);
    }

    // The discriminator: at this exact 200x200 size, a Grid whose rows are literally
    // "24,*" instead of "Auto,*" renders bit-for-bit identical rectangles to the
    // above two tests and would pass them both. This test looks at the Grid's own
    // RowDefinitions/ColumnDefinitions instead of the rendered geometry, so a
    // hard-coded row height (or a hard-coded star column) fails here even though
    // the Bounds-only assertions above cannot tell the difference.
    [AvaloniaFact]
    public void Rows_Use_Auto_And_Star_Sizing_Not_Hard_Coded_Heights()
    {
        var view = Show();
        var grid = view.FindControl<Grid>("RootGrid");
        Assert.NotNull(grid);

        Assert.True(grid!.RowDefinitions[0].Height.IsAuto,
            "row 0 must be Auto so it takes its height from the header content");
        Assert.True(grid.RowDefinitions[1].Height.IsStar,
            "row 1 must be star-sized so it absorbs the remaining height");
        Assert.True(grid.ColumnDefinitions[0].Width.IsAbsolute,
            "column 0 must be a fixed pixel width, not Auto or star");
        Assert.Equal(80, grid.ColumnDefinitions[0].Width.Value);
        Assert.True(grid.ColumnDefinitions[1].Width.IsStar,
            "column 1 must be star-sized so it absorbs the remaining width");
    }
}
