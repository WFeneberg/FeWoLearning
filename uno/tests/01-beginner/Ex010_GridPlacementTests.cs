using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex010_GridPlacementTests : UnoTestContext
{
    private static (Grid Grid, Border Icon, Border Header, Border Body) Badge()
    {
        var icon = new Border();

        // A Border, not a bare TextBlock: a TextBlock arranges to the width of its text,
        // which would hide whether the star column handed it the whole cell. The text
        // inside still gives the Auto row a height to be automatic about.
        var header = new Border { Child = new TextBlock { Text = "Header" } };
        var body = new Border();

        var grid = Ex010_GridPlacement.CreateBadge(icon, header, body);
        return (grid, icon, header, body);
    }

    [Fact]
    public void Declares_Two_Rows_And_Two_Columns()
    {
        var (grid, _, _, _) = Badge();

        Assert.Equal(2, grid.RowDefinitions.Count);
        Assert.Equal(2, grid.ColumnDefinitions.Count);
    }

    [Fact]
    public void Sizes_The_Rows_Auto_Then_Star()
    {
        var (grid, _, _, _) = Badge();

        Assert.Equal(GridUnitType.Auto, grid.RowDefinitions[0].Height.GridUnitType);
        Assert.Equal(GridUnitType.Star, grid.RowDefinitions[1].Height.GridUnitType);
    }

    [Fact]
    public void Sizes_The_Columns_Fixed_Then_Star()
    {
        var (grid, _, _, _) = Badge();

        Assert.Equal(GridUnitType.Pixel, grid.ColumnDefinitions[0].Width.GridUnitType);
        Assert.Equal(40, grid.ColumnDefinitions[0].Width.Value);
        Assert.Equal(GridUnitType.Star, grid.ColumnDefinitions[1].Width.GridUnitType);
    }

    [Fact]
    public void Places_Each_Child_In_Its_Cell()
    {
        var (grid, icon, header, body) = Badge();

        Assert.Equal(3, grid.Children.Count);

        Assert.Equal(0, Grid.GetRow(icon));
        Assert.Equal(0, Grid.GetColumn(icon));

        Assert.Equal(0, Grid.GetRow(header));
        Assert.Equal(1, Grid.GetColumn(header));

        Assert.Equal(1, Grid.GetRow(body));
        Assert.Equal(1, Grid.GetColumn(body));
    }

    [Fact]
    public void The_Star_Column_Takes_Whatever_The_Fixed_One_Left()
    {
        var (grid, icon, header, _) = Badge();

        Layout(grid, width: 200, height: 100);

        Assert.Equal(40, icon.ActualWidth, 1);
        Assert.Equal(160, header.ActualWidth, 1);
    }

    [Fact]
    public void The_Star_Row_Takes_Whatever_The_Auto_One_Left()
    {
        var (grid, _, header, body) = Badge();

        Layout(grid, width: 200, height: 100);

        // Auto is the header's own height, so the body gets the remainder - not half.
        Assert.Equal(100 - header.ActualHeight, body.ActualHeight, 1);
        Assert.True(header.ActualHeight > 0, "the Auto row collapsed");
    }
}
