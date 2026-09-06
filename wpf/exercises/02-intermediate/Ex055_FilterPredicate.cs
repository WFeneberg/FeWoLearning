// Exercise 055 - ICollectionView.Filter and Refresh (intermediate).
// Goal:   ICollectionView.Filter decides which items a view exposes without ever touching the
//         source collection - assigning a NEW predicate to Filter already re-applies it, no
//         Refresh() call needed for that. Refresh() earns its keep for the case assigning a new
//         predicate cannot cover: the SAME predicate delegate's own outcome changing later (some
//         captured value it reads is now different) with no new assignment to Filter at all -
//         WPF has no way to notice that on its own, so the view goes on showing the stale result
//         until something calls Refresh() to force it to re-run the predicate against every item.
// Drills: setting view.Filter to select items - never by removing non-matching items from the
//         source collection - and calling view.Refresh() to force the CURRENT filter predicate
//         to be re-evaluated once its own outcome could have changed without a new delegate
//         being assigned.
// Passes: dotnet test --filter FullyQualifiedName~Ex055_

using System.ComponentModel;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex055_FilterPredicate
{
    /// <summary>
    /// Sets <paramref name="view"/>.Filter to <paramref name="predicate"/> - never by removing
    /// non-matching items from whatever source collection the view was built over.
    /// </summary>
    public static void ApplyFilter(ICollectionView view, Predicate<object> predicate) =>
        throw new NotImplementedException("TODO: Ex055 - assign predicate as view's own Filter - never remove non-matching items from whatever source collection the view was built over instead");

    /// <summary>
    /// Forces <paramref name="view"/> to re-run its CURRENT filter predicate against every item
    /// - via ICollectionView.Refresh() - for the case where the predicate's own outcome changed
    /// without a new delegate ever being assigned to Filter.
    /// </summary>
    public static void ReapplyFilter(ICollectionView view) =>
        throw new NotImplementedException("TODO: Ex055 - force view to re-run its current filter predicate against every item, using view's own refresh mechanism - not a no-op");
}
