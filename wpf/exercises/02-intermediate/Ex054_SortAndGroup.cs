// Exercise 054 - Sorting and grouping through the view, not the source (intermediate).
// Goal:   ICollectionView.SortDescriptions and GroupDescriptions are what actually reorder and
//         bucket what an ItemsControl displays - the source collection itself never moves and
//         is never touched to produce either effect; both collections are observed by the view
//         itself, so adding to them already triggers a re-application with no separate Refresh()
//         call needed (unlike row 055's Filter, whose OWN state can change without WPF having
//         any way to notice on its own).
// Drills: a SortDescription added to view.SortDescriptions (never reordering the source list
//         itself - the source keeps its original insertion order the whole time) and a
//         PropertyGroupDescription added to view.GroupDescriptions (never a Dictionary/lookup
//         you project into yourself - the grouping must be observable through view.Groups).
// Passes: dotnet test --filter FullyQualifiedName~Ex054_

using System.ComponentModel;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// Ready to use - not the subject of this row. A plain view-model item this row sorts and
/// groups by, same shape convention as rows 032/033's items.
/// </summary>
public class Ex054_Item
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
}

public static class Ex054_SortAndGroup
{
    /// <summary>
    /// Sorts <paramref name="view"/> ascending by <paramref name="propertyName"/> - via a
    /// SortDescription added to view.SortDescriptions, never by reordering whatever source
    /// collection the view was built over.
    /// </summary>
    public static void SortAscendingBy(ICollectionView view, string propertyName) =>
        throw new NotImplementedException("TODO: Ex054 - view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending))");

    /// <summary>
    /// Groups <paramref name="view"/> by <paramref name="propertyName"/> - via a
    /// PropertyGroupDescription added to view.GroupDescriptions, never a Dictionary or lookup
    /// you project into yourself; the buckets must be observable through view.Groups.
    /// </summary>
    public static void GroupBy(ICollectionView view, string propertyName) =>
        throw new NotImplementedException("TODO: Ex054 - view.GroupDescriptions!.Add(new PropertyGroupDescription(propertyName))");
}
