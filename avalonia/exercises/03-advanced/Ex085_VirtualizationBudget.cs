using System.Collections.Generic;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 085 - VirtualizationBudget (advanced).
/// Goal:   See what virtualization actually buys, by building the same 500-row
///         list twice - once virtualizing, once not - and comparing how many
///         containers each one realizes. The number is the whole lesson: a
///         viewport's worth against all of them.
/// Drills: ItemsControl.ItemsPanel, VirtualizingStackPanel versus StackPanel,
///         GetRealizedContainers, ContainerFromIndex, FirstRealizedIndex.
/// Passes: dotnet test --filter FullyQualifiedName~Ex085_
///
/// Measured in this harness, with 500 items about 37 units tall each:
///   - a virtualizing list realized 2 containers in a 60-unit viewport, 4 in 120
///     and 9 in 300 - roughly a viewport's worth plus one;
///   - after ScrollIntoView(300) the realized range was 297..300, and
///     ContainerFromIndex(0) had become NULL: the container that used to show the
///     first row was recycled to show a later one;
///   - a plain StackPanel realized all 500 regardless of the viewport.
/// The test asserts the relationships rather than those exact counts, because a
/// row's height depends on font metrics; what it does pin down is that the
/// virtualizing list stays far below the item count while the other does not.
///
/// This is the row where getting it wrong is silent: a list that works perfectly
/// on 20 items and freezes on 20,000 usually lost its virtualizing panel to an
/// innocent-looking ItemsPanel override.
public static class Ex085_VirtualizationBudget
{
    /// <summary>Given. Do not change. What both lists are filled with.</summary>
    public static IReadOnlyList<string> Rows { get; } = BuildRows();

    private static string[] BuildRows()
    {
        var rows = new string[500];

        for (var i = 0; i < rows.Length; i++)
        {
            rows[i] = $"row {i}";
        }

        return rows;
    }

    /// <summary>
    /// A ListBox over Rows, sized to <paramref name="viewportHeight"/>, that
    /// realizes only what it needs. Do not set ItemsPanel at all: a ListBox already
    /// uses a VirtualizingStackPanel, and the exercise is to know that rather than
    /// to build one.
    /// </summary>
    public static ListBox BuildVirtualizing(double viewportHeight) =>
        throw new NotImplementedException(
            "TODO: Ex085 - a ListBox with ItemsSource set to Rows, Width 180 and " +
            "Height viewportHeight, leaving its default panel alone");

    /// <summary>
    /// The same list with virtualization thrown away, so the test can show the
    /// difference. Give it an ItemsPanel of a plain StackPanel - this is exactly
    /// the well-meant override that costs a real application its scrolling
    /// performance.
    /// </summary>
    public static ListBox BuildNonVirtualizing(double viewportHeight) =>
        throw new NotImplementedException(
            "TODO: Ex085 - the same ListBox, but with ItemsPanel set to an " +
            "ItemsPanelTemplate producing a StackPanel. FuncTemplate<Panel> is the " +
            "code-side way to write one");
}
