using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex056_DeferRefreshTests : WpfTestContext
{
    [WpfFact]
    public void ApplyBatched_Collapses_Two_SortDescriptions_Into_One_Reset()
    {
        var items = new ObservableCollection<Ex054_Item>
        {
            new() { Name = "Charlie", Category = "B" },
            new() { Name = "Alpha", Category = "A" },
            new() { Name = "Bravo", Category = "A" },
        };
        var view = CollectionViewSource.GetDefaultView(items);
        var (total, resets) = CountChanges(view);

        Ex056_DeferRefresh.ApplyBatched(view, () =>
        {
            view.SortDescriptions.Add(new SortDescription(nameof(Ex054_Item.Category), ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription(nameof(Ex054_Item.Name), ListSortDirection.Ascending));
        });

        // Against a bypass that never opens a deferred scope at all (just makes the two changes
        // directly): that would show 2 resets here, one per SortDescriptions.Add, not 1.
        Assert.Equal(1, total());
        Assert.Equal(1, resets());
        Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, view.Cast<Ex054_Item>().Select(i => i.Name));
    }

    [WpfFact]
    public void A_Different_Batch_Of_Filter_And_Sort_Also_Collapses_Into_One_Reset()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance - a different
        // collection, a Filter change instead of a second sort key, and a descending sort.
        var items = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };
        var view = CollectionViewSource.GetDefaultView(items);
        var (total, resets) = CountChanges(view);

        Ex056_DeferRefresh.ApplyBatched(view, () =>
        {
            view.Filter = o => (int)o % 2 == 0;
            view.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Descending));
        });

        Assert.Equal(1, total());
        Assert.Equal(1, resets());
        Assert.Equal(new[] { 6, 4, 2 }, view.Cast<int>());
    }

    [WpfFact]
    public void Mutate_Runs_Inside_A_Genuinely_Deferred_Scope()
    {
        // The load-bearing test against the two sneakiest bypasses: disposing the scope
        // immediately after opening it, or calling mutate() outside the scope entirely (before
        // opening it, after disposing it, or around a scope variable that is never actually
        // entered). Measured directly (see wpf/README.md): while a DeferRefresh scope is
        // genuinely open, reading CurrentItem from code running inside it throws
        // InvalidOperationException. A bypass that does not really keep mutate() inside the
        // deferred window never sees that throw.
        var items = new ObservableCollection<int> { 1, 2, 3 };
        var view = CollectionViewSource.GetDefaultView(items);
        var threwInsideScope = false;

        Ex056_DeferRefresh.ApplyBatched(view, () =>
        {
            view.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Descending));
            try
            {
                _ = view.CurrentItem;
            }
            catch (InvalidOperationException)
            {
                threwInsideScope = true;
            }
        });

        Assert.True(threwInsideScope, "reading the view's CurrentItem from inside mutate() should have thrown - mutate did not actually run inside a deferred DeferRefresh scope");
    }

    private static (Func<int> Total, Func<int> Resets) CountChanges(ICollectionView view)
    {
        var total = 0;
        var resets = 0;
        ((INotifyCollectionChanged)view).CollectionChanged += (_, e) =>
        {
            total++;
            if (e.Action == NotifyCollectionChangedAction.Reset) resets++;
        };
        return (() => total, () => resets);
    }
}
