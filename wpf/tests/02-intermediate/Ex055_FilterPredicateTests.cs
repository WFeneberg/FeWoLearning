using System.Collections.ObjectModel;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex055_FilterPredicateTests : WpfTestContext
{
    [WpfFact]
    public void ApplyFilter_Applies_Immediately_Without_Touching_The_Source()
    {
        var items = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        var view = CollectionViewSource.GetDefaultView(items);

        Ex055_FilterPredicate.ApplyFilter(view, o => (int)o % 2 == 0);

        // Measured directly (see wpf/README.md): assigning a NEW predicate to Filter already
        // re-applies it - no Refresh() call needed here for THIS to show up.
        Assert.Equal(new[] { 2, 4 }, view.Cast<int>());

        // Against a bypass that filters by removing non-matching items from the source
        // collection itself instead of setting Filter: the source must be untouched.
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, items);
    }

    [WpfFact]
    public void A_Different_Collection_And_Predicate_Also_Filters_Via_The_View_Only()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var items = new ObservableCollection<string> { "aa", "b", "ccc", "d", "eee" };
        var view = CollectionViewSource.GetDefaultView(items);

        Ex055_FilterPredicate.ApplyFilter(view, o => ((string)o).Length > 1);

        Assert.Equal(new[] { "aa", "ccc", "eee" }, view.Cast<string>());
        Assert.Equal(new[] { "aa", "b", "ccc", "d", "eee" }, items);
    }

    [WpfFact]
    public void ReapplyFilter_Forces_ReEvaluation_When_The_Predicates_Own_Captured_State_Changed()
    {
        // The row's actual subject: the SAME predicate delegate, never reassigned to Filter a
        // second time, changes its own answer because a value it captures changed elsewhere -
        // measured directly that WPF has no way to notice this on its own.
        var items = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        var view = CollectionViewSource.GetDefaultView(items);
        var threshold = 2;
        bool Predicate(object o) => (int)o > threshold;

        Ex055_FilterPredicate.ApplyFilter(view, Predicate);
        Assert.Equal(new[] { 3, 4, 5 }, view.Cast<int>());

        threshold = 4; // the predicate's own outcome changed - no new Filter assignment at all

        // Against a bypass whose ReapplyFilter is a no-op (or whose ApplyFilter secretly keeps
        // re-refreshing on its own, which would already have shown [5] here, before Reapply was
        // even called): still the STALE result until Reapply actually runs.
        Assert.Equal(new[] { 3, 4, 5 }, view.Cast<int>());

        Ex055_FilterPredicate.ReapplyFilter(view);

        Assert.Equal(new[] { 5 }, view.Cast<int>());
    }

    [WpfFact]
    public void ReapplyFilter_With_A_Different_Threshold_Also_ReEvaluates()
    {
        var items = new ObservableCollection<int> { 10, 20, 30, 40 };
        var view = CollectionViewSource.GetDefaultView(items);
        var limit = 15;
        bool Predicate(object o) => (int)o < limit;

        Ex055_FilterPredicate.ApplyFilter(view, Predicate);
        Assert.Equal(new[] { 10 }, view.Cast<int>());

        limit = 35;
        Ex055_FilterPredicate.ReapplyFilter(view);

        Assert.Equal(new[] { 10, 20, 30 }, view.Cast<int>());
    }
}
