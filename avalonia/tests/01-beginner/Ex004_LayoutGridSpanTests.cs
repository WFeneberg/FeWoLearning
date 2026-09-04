using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex004_LayoutGridSpanTests
{
    // 400 wide over 1*/2*/1* gives 100 / 200 / 100.
    private static Ex004_LayoutGridSpan Show() =>
        ViewHarness.Show(new Ex004_LayoutGridSpan(), 400, 200);

    [AvaloniaFact]
    public void Banner_Spans_The_Full_Width_Of_All_Three_Columns()
    {
        var view = Show();

        Assert.Equal(new Rect(0, 0, 400, 16), view.FindControl<Border>("Banner")!.Bounds);
    }

    // The discriminator: three equal columns would give 133.33 each and fail.
    [AvaloniaFact]
    public void Middle_Column_Is_Exactly_Twice_Each_Outer_Column()
    {
        var view = Show();

        var left = view.FindControl<Border>("Left")!;
        var middle = view.FindControl<Border>("Middle")!;
        var right = view.FindControl<Border>("Right")!;

        Assert.Equal(new Rect(0, 16, 100, 30), left.Bounds);
        Assert.Equal(new Rect(100, 16, 200, 30), middle.Bounds);
        Assert.Equal(new Rect(300, 16, 100, 30), right.Bounds);
        Assert.Equal(2 * left.Bounds.Width, middle.Bounds.Width);
    }

    // The discriminator: fixed columns of 100/200/100 render bit-for-bit identical
    // rectangles to both tests above at this exact 400x200 host size - even the
    // "twice as wide" assertion holds for hard-coded pixels that happen to be in a
    // 1:2:1 ratio. This test looks at the Grid's own ColumnDefinitions instead of
    // the rendered geometry, so hard-coded pixel widths fail here even though the
    // Bounds-only assertions above cannot tell the difference.
    [AvaloniaFact]
    public void Columns_Use_Star_Sizing_In_A_One_Two_One_Ratio_And_Banner_Spans_All_Three()
    {
        var view = Show();
        var grid = view.FindControl<Grid>("RootGrid");
        Assert.NotNull(grid);

        Assert.True(grid!.ColumnDefinitions[0].Width.IsStar,
            "column 0 must be star-sized, not a fixed pixel width");
        Assert.Equal(1, grid.ColumnDefinitions[0].Width.Value);
        Assert.True(grid.ColumnDefinitions[1].Width.IsStar,
            "column 1 must be star-sized, not a fixed pixel width");
        Assert.Equal(2, grid.ColumnDefinitions[1].Width.Value);
        Assert.True(grid.ColumnDefinitions[2].Width.IsStar,
            "column 2 must be star-sized, not a fixed pixel width");
        Assert.Equal(1, grid.ColumnDefinitions[2].Width.Value);

        var banner = view.FindControl<Border>("Banner")!;
        Assert.Equal(3, Grid.GetColumnSpan(banner));
    }
}
