// Exercise 056 - DeferRefresh: batching several view changes into one refresh (intermediate). REFERENCE SOLUTION.
// Goal:   Several SortDescriptions/GroupDescriptions/Filter changes each re-apply the moment
//         they are made (rows 054/055) - fine for one change, wasteful for several in a row,
//         since each one re-sorts/re-filters the whole view on its own. ICollectionView.
//         DeferRefresh() suspends that automatic re-application for as long as the returned
//         scope stays open, so several changes collapse into the single Refresh its Dispose
//         finally runs - never one refresh per change.
// Drills: opening view.DeferRefresh() as a using scope around several view-level changes so
//         they collapse into ONE Refresh (observable as a single Reset on the view's own
//         CollectionChanged, never one Reset per change), and that the view genuinely treats
//         itself as mid-refresh for the whole scope: reading CurrentItem (or enumerating the
//         view) from CODE running INSIDE that scope throws InvalidOperationException - measured
//         directly, not assumed - so mutate must actually run inside the deferred window, not
//         merely be called somewhere near it.

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex056_DeferRefresh
{
    /// <summary>
    /// Runs <paramref name="mutate"/> while <paramref name="view"/>'s Refresh is deferred, so
    /// however many view-level changes it makes (SortDescriptions, GroupDescriptions, Filter, ...)
    /// collapse into the single Refresh that firing the scope's Dispose runs - instead of one
    /// Refresh per change.
    /// </summary>
    public static void ApplyBatched(ICollectionView view, Action mutate)
    {
        using (view.DeferRefresh())
        {
            mutate();
        }
    }
}
