// Exercise 029 - Star, Auto and pixel column sizing (beginner).
// Goal:   Build a Grid whose three columns use the three different GridUnitTypes - Auto
//         (sized to content), Star (a share of whatever space remains), and a fixed pixel
//         width - and see that the definitions themselves, not just the resulting
//         rectangles, are what actually distinguishes them: a fixed-width column and an
//         Auto column can render at the same width by coincidence, but only one of them
//         reports GridUnitType.Auto.
// Drills: ColumnDefinition.Width as a GridLength (GridUnitType.Auto / .Star / .Pixel, plus
//         the numeric Value each one carries), and Grid's own column-measure pass: Auto
//         shrinks to its content's desired width, Pixel never moves regardless of content,
//         Star absorbs whatever width neither of the other two claimed.
// Passes: dotnet test --filter FullyQualifiedName~Ex029_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex029_GridStarAndAuto
{
    /// <summary>
    /// Builds a 3-column Grid: column 0 is Auto, column 1 is Star with factor 2 (deliberately
    /// not 1 - ColumnDefinition's own unassigned default is ALSO Star(1), so this column has
    /// to be assigned explicitly to read back as 2), column 2 is a fixed Pixel width of
    /// <paramref name="pixelColumnWidth"/>.
    /// </summary>
    public static Grid BuildGrid(double pixelColumnWidth)
        // TODO: var grid = new Grid();
        //       grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
        //       grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
        //       grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pixelColumnWidth, GridUnitType.Pixel) });
        //       return grid;
        => throw new NotImplementedException("TODO: Ex029 - build a Grid with three ColumnDefinitions in order: Width = new GridLength(1, GridUnitType.Auto), Width = new GridLength(2, GridUnitType.Star), Width = new GridLength(pixelColumnWidth, GridUnitType.Pixel)");
}
