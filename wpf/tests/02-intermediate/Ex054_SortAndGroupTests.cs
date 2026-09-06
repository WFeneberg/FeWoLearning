using System.Collections.ObjectModel;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex054_SortAndGroupTests : WpfTestContext
{
    [WpfFact]
    public void SortAscendingBy_Orders_The_View_Without_Reordering_The_Source()
    {
        var items = new ObservableCollection<Ex054_Item>
        {
            new() { Name = "Charlie", Category = "B" },
            new() { Name = "Alpha", Category = "A" },
            new() { Name = "Bravo", Category = "A" },
        };
        var view = CollectionViewSource.GetDefaultView(items);

        Ex054_SortAndGroup.SortAscendingBy(view, nameof(Ex054_Item.Name));

        var viewOrder = view.Cast<Ex054_Item>().Select(i => i.Name).ToArray();
        Assert.Equal(new[] { "Alpha", "Bravo", "Charlie" }, viewOrder);

        // Against a bypass that sorts by reordering the source list itself instead of adding a
        // SortDescription: the source must keep its ORIGINAL insertion order the whole time.
        Assert.Equal(new[] { "Charlie", "Alpha", "Bravo" }, items.Select(i => i.Name));
        Assert.Single(view.SortDescriptions);
        Assert.Equal(nameof(Ex054_Item.Name), view.SortDescriptions[0].PropertyName);
    }

    [WpfFact]
    public void A_Different_Collection_And_Property_Also_Sorts_Via_The_View_Only()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var items = new ObservableCollection<Ex054_Item>
        {
            new() { Name = "Zed", Category = "Z" },
            new() { Name = "Mid", Category = "M" },
            new() { Name = "Ay", Category = "A" },
        };
        var view = CollectionViewSource.GetDefaultView(items);

        Ex054_SortAndGroup.SortAscendingBy(view, nameof(Ex054_Item.Category));

        Assert.Equal(new[] { "A", "M", "Z" }, view.Cast<Ex054_Item>().Select(i => i.Category));
        Assert.Equal(new[] { "Z", "M", "A" }, items.Select(i => i.Category));
    }

    [WpfFact]
    public void GroupBy_Buckets_Are_Observable_Through_View_Groups()
    {
        var items = new ObservableCollection<Ex054_Item>
        {
            new() { Name = "Charlie", Category = "B" },
            new() { Name = "Alpha", Category = "A" },
            new() { Name = "Bravo", Category = "A" },
        };
        var view = CollectionViewSource.GetDefaultView(items);

        Ex054_SortAndGroup.GroupBy(view, nameof(Ex054_Item.Category));

        // Against a bypass that groups by projecting into a Dictionary/lookup of its own
        // instead of adding a PropertyGroupDescription: view.GroupDescriptions/view.Groups
        // would never reflect it at all.
        Assert.Single(view.GroupDescriptions!);
        Assert.NotNull(view.Groups);
        Assert.Equal(2, view.Groups!.Count);

        var groupsByName = view.Groups!.Cast<CollectionViewGroup>().ToDictionary(g => (string)g.Name, g => g.ItemCount);
        Assert.Equal(1, groupsByName["B"]);
        Assert.Equal(2, groupsByName["A"]);
    }

    [WpfFact]
    public void A_Different_Grouping_Property_Also_Buckets_Correctly()
    {
        var items = new ObservableCollection<Ex054_Item>
        {
            new() { Name = "One", Category = "X" },
            new() { Name = "Two", Category = "Y" },
            new() { Name = "Three", Category = "X" },
            new() { Name = "Four", Category = "Y" },
            new() { Name = "Five", Category = "X" },
        };
        var view = CollectionViewSource.GetDefaultView(items);

        // A genuinely DIFFERENT property than the test above - Name, not Category. The
        // previous version of this test passed the SAME property (Category) both times, so
        // the parameter never actually varied and a hardcoded "Category" string satisfied it
        // undetected. Name is distinct per item here, producing five single-item buckets -
        // Category's two-bucket split cannot satisfy this instead.
        Ex054_SortAndGroup.GroupBy(view, nameof(Ex054_Item.Name));

        var groupNames = view.Groups!.Cast<CollectionViewGroup>().Select(g => (string)g.Name).OrderBy(n => n).ToArray();
        Assert.Equal(new[] { "Five", "Four", "One", "Three", "Two" }, groupNames);
        Assert.All(view.Groups!.Cast<CollectionViewGroup>(), g => Assert.Equal(1, g.ItemCount));
    }
}
