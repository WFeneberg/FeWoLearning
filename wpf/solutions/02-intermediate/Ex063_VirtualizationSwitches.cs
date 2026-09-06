// Exercise 063 - The virtualization switches, and what they actually need to sit on (intermediate). REFERENCE SOLUTION.
// Goal:   VirtualizingPanel.IsVirtualizing, VirtualizationMode and ScrollUnit are attached
//         properties, but measured directly - not assumed from the "attached property" label -
//         their own metadata does NOT set FrameworkPropertyMetadataOptions.Inherits, so setting
//         them on some ancestor and hoping the value cascades down to the panel does nothing.
//         What actually works, also measured directly: setting them on the ItemsControl whose
//         default items panel is a VirtualizingStackPanel in the first place - a ListBox/ListView,
//         not a bare ItemsControl, whose own default panel is a plain, non-virtualizing StackPanel
//         that ignores these properties entirely regardless of what is set on it (see README,
//         "Initialization": the same ListBox-vs-ItemsControl default-panel distinction rows
//         031-034 already depend on). This row owns the SWITCHES and what they declare - the
//         Concepts cell's own VirtualizationMode is shared with row 076 (ContainerRecycling),
//         which owns container IDENTITY across an actual scroll instead; this row never scrolls
//         anything and asserts no container identity at all.
// Drills: VirtualizingPanel.SetIsVirtualizing/SetVirtualizationMode/SetScrollUnit, applied to the
//         right kind of ItemsControl, and reading them back the same way. Measured directly with
//         200 items in an 800x600-capable but viewport-bounded ListBox: leaving IsVirtualizing at
//         its (already-true) default realizes only a double-digit handful of containers;
//         explicitly setting it false realizes every single one of the 200 - virtualization is
//         genuinely observable here, no window or real scrolling needed.

using System.Collections;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex063_VirtualizationSwitches
{
    /// <summary>
    /// Builds an ItemsControl bound to <paramref name="items"/> with the three virtualization
    /// switches applied to it directly. Must be (or behave like) a ListBox - an ItemsControl whose
    /// default items panel is actually a VirtualizingStackPanel - not a bare ItemsControl, whose
    /// default panel is a plain StackPanel that silently ignores every one of these switches.
    /// </summary>
    public static ItemsControl BuildVirtualizedList(IEnumerable items, bool isVirtualizing, VirtualizationMode mode, ScrollUnit scrollUnit)
    {
        var list = new ListBox { ItemsSource = items };

        VirtualizingPanel.SetIsVirtualizing(list, isVirtualizing);
        VirtualizingPanel.SetVirtualizationMode(list, mode);
        VirtualizingPanel.SetScrollUnit(list, scrollUnit);

        return list;
    }
}
