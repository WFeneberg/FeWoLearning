// Exercise 053 - CollectionViewSource basics: ICollectionView, CurrentItem, MoveCurrentTo (intermediate). REFERENCE SOLUTION.
// Goal:   A bound ItemsControl never enumerates a source collection directly - WPF's own
//         binding engine always goes through the ICollectionView that
//         CollectionViewSource.GetDefaultView builds for it, and that view is also what owns
//         "the current item" (CurrentItem/CurrentPosition): one shared position over the
//         collection, not something each consumer computes for itself.
// Drills: CollectionViewSource.GetDefaultView returning the SAME view WPF's own binding engine
//         would use for that collection (not a fresh wrapper of your own around the source),
//         and ICollectionView.MoveCurrentTo as the one way to change CurrentItem/CurrentPosition
//         - never index arithmetic against the source collection, which only ever produces a
//         value you compute yourself and leaves the view's own CurrentItem/CurrentPosition
//         exactly where they were.

using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex053_CollectionViewSourceBasics
{
    /// <summary>
    /// Returns the ICollectionView WPF's own binding engine would use for
    /// <paramref name="items"/> - via CollectionViewSource.GetDefaultView(items), not a view
    /// type of your own construction.
    /// </summary>
    public static ICollectionView GetDefaultView(IEnumerable items) =>
        CollectionViewSource.GetDefaultView(items);

    /// <summary>
    /// Moves <paramref name="view"/>'s current position to <paramref name="item"/> via
    /// ICollectionView.MoveCurrentTo - never by computing an index against the source
    /// collection yourself - and returns view.CurrentItem afterward (null if the move failed,
    /// which MoveCurrentTo itself already reports via its own return value and via
    /// CurrentPosition becoming -1).
    /// </summary>
    public static object? MoveToItem(ICollectionView view, object item)
    {
        view.MoveCurrentTo(item);
        return view.CurrentItem;
    }
}
