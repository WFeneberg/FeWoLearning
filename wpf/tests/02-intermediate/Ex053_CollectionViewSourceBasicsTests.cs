using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex053_CollectionViewSourceBasicsTests : WpfTestContext
{
    [WpfFact]
    public void GetDefaultView_Returns_The_Same_View_WPFs_Own_Binding_Engine_Would_Use()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };

        var view = Ex053_CollectionViewSourceBasics.GetDefaultView(items);

        // Against a bypass that hands back the source collection itself (or some fresh wrapper
        // of its own) instead of the real, cached default view: this must be the EXACT same
        // instance CollectionViewSource.GetDefaultView(items) itself returns for this collection.
        Assert.Same(CollectionViewSource.GetDefaultView(items), view);
        Assert.NotSame(items, view);
        Assert.IsAssignableFrom<ICollectionView>(view);
    }

    [WpfFact]
    public void MoveToItem_Updates_The_Views_Own_CurrentItem_And_CurrentPosition()
    {
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var view = Ex053_CollectionViewSourceBasics.GetDefaultView(items);

        var current = Ex053_CollectionViewSourceBasics.MoveToItem(view, "c");

        Assert.Equal("c", current);
        // Load-bearing against index arithmetic against the SOURCE that merely computes and
        // returns "c" without ever touching the view itself: the view's own state must have
        // actually moved.
        Assert.Equal("c", view.CurrentItem);
        Assert.Equal(2, view.CurrentPosition);
    }

    [WpfFact]
    public void A_Different_Collection_And_Target_Also_Move_The_Views_Own_Position()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance - a hardcoded
        // position 2 from the test above cannot satisfy this one too.
        var items = new ObservableCollection<string> { "x", "y", "z", "w" };
        var view = Ex053_CollectionViewSourceBasics.GetDefaultView(items);

        var current = Ex053_CollectionViewSourceBasics.MoveToItem(view, "y");

        Assert.Equal("y", current);
        Assert.Equal("y", view.CurrentItem);
        Assert.Equal(1, view.CurrentPosition);
    }

    [WpfFact]
    public void Moving_To_An_Item_Not_In_The_Collection_Leaves_No_Current_Item()
    {
        // Measured directly: ICollectionView.MoveCurrentTo with an item it cannot find sets
        // CurrentPosition to -1 and CurrentItem to null - a bypass that just echoes back
        // whatever item it was given (never actually calling MoveCurrentTo) would return the
        // bogus item here instead.
        var items = new ObservableCollection<string> { "a", "b" };
        var view = Ex053_CollectionViewSourceBasics.GetDefaultView(items);
        Ex053_CollectionViewSourceBasics.MoveToItem(view, "a");

        var current = Ex053_CollectionViewSourceBasics.MoveToItem(view, "not-in-the-collection");

        Assert.Null(current);
        Assert.Equal(-1, view.CurrentPosition);
    }
}
