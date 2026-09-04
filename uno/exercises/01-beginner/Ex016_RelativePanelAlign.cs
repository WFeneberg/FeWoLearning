// Exercise 016 - Relative Panel Align (beginner).
// Goal:   Position siblings by describing their relationships instead of their coordinates.
// Drills: RelativePanel.RightOf/Below/AlignTopWith/AlignLeftWith, the default top-left
//         anchor, and how the panel's own DesiredSize follows from the solved constraints.
// Passes: dotnet test --filter FullyQualifiedName~Ex016_
//
// A RelativePanel solves a small constraint system at measure time. It is the one panel
// where a child's position depends on another child rather than on its index.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex016_RelativePanelAlign
{
    /// <summary>
    /// A badge built from three relationships, all children added in this order:
    /// <list type="bullet">
    ///   <item><paramref name="icon"/> unconstrained, so it anchors at the top left,</item>
    ///   <item><paramref name="title"/> to the right of the icon and top-aligned with it,</item>
    ///   <item><paramref name="subtitle"/> below the title and left-aligned with it.</item>
    /// </list>
    /// </summary>
    public static RelativePanel CreateBadge(FrameworkElement icon, FrameworkElement title, FrameworkElement subtitle) =>
        // TODO: create the panel, add the three children, and set the four attached
        // relationships. Do not set any Margin or Canvas position - the point is that no
        // coordinate appears anywhere in this method.
        throw new NotImplementedException("TODO: Ex016 - relate the three children to each other");
}
