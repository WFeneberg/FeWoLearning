// Exercise 014 - Alignment And Stretch (beginner).
// Goal:   Position a child inside a cell that is bigger than it is.
// Drills: HorizontalAlignment/VerticalAlignment, Stretch as the default, and the rule that
//         catches everybody: Stretch plus an explicit Width or Height centres the element
//         instead of filling the slot.
// Passes: dotnet test --filter FullyQualifiedName~Ex014_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex014_AlignmentStretch
{
    /// <summary>
    /// A single-cell Grid holding <paramref name="child"/> with the requested alignments.
    /// The Grid itself is given no rows or columns, so the child gets the whole surface to
    /// be aligned in.
    /// </summary>
    public static Grid CreateCell(FrameworkElement child, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
        // Alignment lives on the child: it says how the child wants to sit in whatever slot
        // its parent hands it. The parent is not consulted, which is why the same child
        // behaves identically in a Grid cell, a Border or a ContentControl.
        child.HorizontalAlignment = horizontal;
        child.VerticalAlignment = vertical;

        var grid = new Grid();
        grid.Children.Add(child);
        return grid;
    }
}
