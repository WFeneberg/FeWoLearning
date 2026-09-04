// Exercise 016 - Relative Panel Align (beginner).
// Goal:   Position siblings by describing their relationships instead of their coordinates.
// Drills: RelativePanel.RightOf/Below/AlignTopWith/AlignLeftWith, the default top-left
//         anchor, and how the panel's own DesiredSize follows from the solved constraints.
// Passes: dotnet test --filter FullyQualifiedName~Ex016_

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
    public static RelativePanel CreateBadge(FrameworkElement icon, FrameworkElement title, FrameworkElement subtitle)
    {
        var panel = new RelativePanel();

        panel.Children.Add(icon);
        panel.Children.Add(title);
        panel.Children.Add(subtitle);

        // The attached values hold the sibling *elements*, so the panel re-solves the
        // positions whenever any of them changes size. That is the whole trade: one
        // constraint solve per measure, and not a single coordinate to keep in sync.
        RelativePanel.SetRightOf(title, icon);
        RelativePanel.SetAlignTopWith(title, icon);

        RelativePanel.SetBelow(subtitle, title);
        RelativePanel.SetAlignLeftWith(subtitle, title);

        return panel;
    }
}
