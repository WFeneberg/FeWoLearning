using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex029_GridStarAndAutoTests : WpfTestContext
{
    [WpfFact]
    public void BuildGrid_Has_Three_Columns_With_The_Right_GridUnitTypes_And_Values()
    {
        var grid = Ex029_GridStarAndAuto.BuildGrid(70.0);

        Assert.Equal(3, grid.ColumnDefinitions.Count);

        var auto = grid.ColumnDefinitions[0].Width;
        Assert.Equal(GridUnitType.Auto, auto.GridUnitType);

        var star = grid.ColumnDefinitions[1].Width;
        Assert.Equal(GridUnitType.Star, star.GridUnitType);
        // Deliberately not 1.0 - ColumnDefinition's own unassigned default is ALSO Star(1),
        // measured directly on this machine, so this value is what tells "explicitly
        // assigned Star(2)" apart from "never touched, still the default". This one
        // assertion is the row's entire defence for the star factor: with a single star
        // column absorbing the remainder, Star(1) and Star(2) produce identical geometry,
        // so no rectangle-based test anywhere in this file could catch a wrong factor - it
        // cannot be strengthened by adding a second star column here without changing what
        // the row is about.
        Assert.Equal(2.0, star.Value);

        var pixel = grid.ColumnDefinitions[2].Width;
        Assert.Equal(GridUnitType.Pixel, pixel.GridUnitType);
        Assert.Equal(70.0, pixel.Value);
    }

    [WpfFact]
    public void A_Different_Pixel_Width_Comes_Through_Unchanged()
    {
        // Different call site, different width - a hard-coded 70.0 cannot satisfy both.
        var grid = Ex029_GridStarAndAuto.BuildGrid(130.0);

        Assert.Equal(130.0, grid.ColumnDefinitions[2].Width.Value);
        Assert.Equal(GridUnitType.Pixel, grid.ColumnDefinitions[2].Width.GridUnitType);
    }

    [WpfFact]
    public void Auto_Shrinks_To_Content_Pixel_Stays_Fixed_Star_Takes_The_Remainder()
    {
        var grid = Ex029_GridStarAndAuto.BuildGrid(70.0);
        var autoContent = new Border { Width = 45, Height = 1 };
        Grid.SetColumn(autoContent, 0);
        var starContent = new Border { Height = 1 };
        Grid.SetColumn(starContent, 1);
        grid.Children.Add(autoContent);
        grid.Children.Add(starContent);

        Layout(grid, new Size(300, 50));

        Assert.Equal(45.0, grid.ColumnDefinitions[0].ActualWidth);
        Assert.Equal(70.0, grid.ColumnDefinitions[2].ActualWidth);
        Assert.Equal(185.0, grid.ColumnDefinitions[1].ActualWidth); // 300 - 45 - 70
    }

    [WpfFact]
    public void A_Different_Layout_Produces_Different_Rectangles_From_The_Same_Mechanism()
    {
        // Different pixel width, different Auto content width and available width than the
        // test above - no single hard-coded set of rectangles satisfies both.
        var grid = Ex029_GridStarAndAuto.BuildGrid(130.0);
        var autoContent = new Border { Width = 60, Height = 1 };
        Grid.SetColumn(autoContent, 0);
        var starContent = new Border { Height = 1 };
        Grid.SetColumn(starContent, 1);
        grid.Children.Add(autoContent);
        grid.Children.Add(starContent);

        Layout(grid, new Size(500, 50));

        Assert.Equal(60.0, grid.ColumnDefinitions[0].ActualWidth);
        Assert.Equal(130.0, grid.ColumnDefinitions[2].ActualWidth);
        Assert.Equal(310.0, grid.ColumnDefinitions[1].ActualWidth); // 500 - 60 - 130
    }
}
