using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex063_VirtualizationSwitchesTests : WpfTestContext
{
    private static readonly Size Viewport = new(300, 200);

    private static List<string> BuildItems(int count) => Enumerable.Range(0, count).Select(i => $"item {i}").ToList();

    private static int CountRealized(ItemsControl control, int itemCount)
    {
        var realized = 0;
        for (var i = 0; i < itemCount; i++)
        {
            if (control.ItemContainerGenerator.ContainerFromIndex(i) is not null)
            {
                realized++;
            }
        }

        return realized;
    }

    [WpfFact]
    public void Virtualizing_With_200_Items_Realizes_Far_Fewer_Containers_Than_A_Bounded_Viewport_Holds()
    {
        var items = BuildItems(200);
        var list = Ex063_VirtualizationSwitches.BuildVirtualizedList(items, isVirtualizing: true, VirtualizationMode.Standard, ScrollUnit.Item);

        CompleteInitialization(list);
        Layout(list, Viewport);
        Pump();

        // Measured directly (see README): a 300x200 viewport realizes roughly a dozen of the 200
        // items when virtualizing. This also rejects the "wrong element" mutant: a plain
        // ItemsControl's default (non-virtualizing) StackPanel would realize every one of the 200
        // regardless of what these switches were set to.
        var realized = CountRealized(list, items.Count);
        Assert.True(realized < 100, $"expected far fewer than 200 realized containers with virtualization on, got {realized}");
        Assert.True(realized > 0);
    }

    [WpfFact]
    public void Disabling_IsVirtualizing_Realizes_Every_Item_Even_With_A_Bounded_Viewport()
    {
        var items = BuildItems(200);
        var list = Ex063_VirtualizationSwitches.BuildVirtualizedList(items, isVirtualizing: false, VirtualizationMode.Standard, ScrollUnit.Item);

        CompleteInitialization(list);
        Layout(list, Viewport);
        Pump();

        Assert.Equal(items.Count, CountRealized(list, items.Count));
    }

    [WpfFact]
    public void All_Three_Switches_Are_Declared_On_The_ItemsControl_Itself()
    {
        var items = BuildItems(50);

        // Every value here is away from its own registered default EXCEPT isVirtualizing (true
        // already IS the default) - so a mutant dropping SetVirtualizationMode or SetScrollUnit is
        // caught right here, but one dropping SetIsVirtualizing is not (true is what it would
        // still read back either way); that one is caught instead by the tests above/below, which
        // both pass isVirtualizing: false - away from the default - for exactly this reason.
        var list = Ex063_VirtualizationSwitches.BuildVirtualizedList(items, isVirtualizing: true, VirtualizationMode.Recycling, ScrollUnit.Pixel);

        Assert.True(VirtualizingPanel.GetIsVirtualizing(list));
        Assert.Equal(VirtualizationMode.Recycling, VirtualizingPanel.GetVirtualizationMode(list));
        Assert.Equal(ScrollUnit.Pixel, VirtualizingPanel.GetScrollUnit(list));
    }

    [WpfFact]
    public void A_Third_Combination_Of_Switches_Is_Also_Declared_Correctly()
    {
        // A second, differently-shaped combination from the previous test - vary inputs across
        // call sites rather than trusting one combination to prove all three switches work.
        var items = BuildItems(30);

        var list = Ex063_VirtualizationSwitches.BuildVirtualizedList(items, isVirtualizing: false, VirtualizationMode.Standard, ScrollUnit.Pixel);

        Assert.False(VirtualizingPanel.GetIsVirtualizing(list));
        Assert.Equal(VirtualizationMode.Standard, VirtualizingPanel.GetVirtualizationMode(list));
        Assert.Equal(ScrollUnit.Pixel, VirtualizingPanel.GetScrollUnit(list));
    }

    [WpfFact]
    public void BuildVirtualizedList_Returns_A_ListBox_Whose_Default_Panel_Actually_Virtualizes()
    {
        var items = BuildItems(10);

        var list = Ex063_VirtualizationSwitches.BuildVirtualizedList(items, isVirtualizing: true, VirtualizationMode.Standard, ScrollUnit.Item);

        // The Goal comment's own point, made structural: a bare ItemsControl's default panel is a
        // plain, non-virtualizing StackPanel - this row's whole subject requires an ItemsControl
        // whose default panel is a VirtualizingStackPanel instead (a ListBox, concretely).
        Assert.IsAssignableFrom<ListBox>(list);
    }
}
