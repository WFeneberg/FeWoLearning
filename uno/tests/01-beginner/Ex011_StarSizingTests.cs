using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex011_StarSizingTests : UnoTestContext
{
    private static (Grid Grid, Border Icon, Border Main, Border Aside) Row(double iconWidth = 40)
    {
        var icon = new Border { Width = iconWidth };
        var main = new Border();
        var aside = new Border();
        return (Ex011_StarSizing.CreateRatioRow(icon, main, aside), icon, main, aside);
    }

    [Fact]
    public void Declares_The_Three_Column_Kinds()
    {
        var (grid, _, _, _) = Row();

        Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.Equal(GridUnitType.Auto, grid.ColumnDefinitions[0].Width.GridUnitType);
        Assert.Equal(GridUnitType.Star, grid.ColumnDefinitions[1].Width.GridUnitType);
        Assert.Equal(GridUnitType.Star, grid.ColumnDefinitions[2].Width.GridUnitType);
    }

    [Fact]
    public void Weights_The_Star_Columns_Two_To_One()
    {
        var (grid, _, _, _) = Row();

        Assert.Equal(2, grid.ColumnDefinitions[1].Width.Value);
        Assert.Equal(1, grid.ColumnDefinitions[2].Width.Value);
    }

    [Fact]
    public void Splits_What_The_Auto_Column_Left_Over()
    {
        var (grid, icon, main, aside) = Row();

        Layout(grid, width: 340, height: 50);

        // 340 - 40 for the icon leaves 300, split 2:1.
        Assert.Equal(40, icon.ActualWidth, 1);
        Assert.Equal(200, main.ActualWidth, 1);
        Assert.Equal(100, aside.ActualWidth, 1);
    }

    [Fact]
    public void Star_Is_A_Weight_Not_A_Percentage()
    {
        var (grid, icon, main, aside) = Row();

        Layout(grid, width: 190, height: 50);

        // Same 2:1 ratio out of a different leftover - no fixed percentages anywhere.
        Assert.Equal(40, icon.ActualWidth, 1);
        Assert.Equal(100, main.ActualWidth, 1);
        Assert.Equal(50, aside.ActualWidth, 1);
    }

    [Fact]
    public void The_Auto_Column_Follows_Its_Content()
    {
        var (grid, icon, main, aside) = Row(iconWidth: 80);

        Layout(grid, width: 380, height: 50);

        Assert.Equal(80, icon.ActualWidth, 1);
        Assert.Equal(200, main.ActualWidth, 1);
        Assert.Equal(100, aside.ActualWidth, 1);
    }

    [Fact]
    public void Puts_Each_Child_In_Its_Own_Column()
    {
        var (grid, icon, main, aside) = Row();

        Assert.Equal(3, grid.Children.Count);
        Assert.Equal(0, Grid.GetColumn(icon));
        Assert.Equal(1, Grid.GetColumn(main));
        Assert.Equal(2, Grid.GetColumn(aside));
    }
}
