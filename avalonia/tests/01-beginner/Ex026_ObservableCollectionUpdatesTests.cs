using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex026_ObservableCollectionUpdatesTests
{
    private static (Ex026_ObservableCollectionUpdates View, Ex026_ObservableCollectionUpdatesViewModel Vm) Arrange()
    {
        var vm = new Ex026_ObservableCollectionUpdatesViewModel();
        var view = ViewHarness.Show(new Ex026_ObservableCollectionUpdates { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    private static List<string?> Texts(ItemsControl itemsControl) =>
        itemsControl.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();

    // Mechanism check: a control named ItemList that is not actually an
    // ItemsControl (e.g. a hard-coded StackPanel given the same name) fails
    // this typed lookup outright.
    [AvaloniaFact]
    public void ItemList_Is_An_ItemsControl_Bound_To_The_Vms_Items()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("ItemList");

        Assert.NotNull(itemsControl);
        Assert.Same(vm.Items, itemsControl!.ItemsSource);
        Assert.NotNull(itemsControl.ItemTemplate);
    }

    [AvaloniaFact]
    public void Initial_Render_Shows_Both_Starting_Items()
    {
        var (view, _) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("ItemList")!;

        Assert.Equal(2, itemsControl.GetRealizedContainers().Count());
        var texts = Texts(itemsControl);
        Assert.Contains("Alpha", texts);
        Assert.Contains("Beta", texts);
    }

    // The real discriminator against a hard-coded panel: the rendered list
    // must react to BOTH an addition and a removal on the live collection,
    // not just show the right thing once at startup.
    [AvaloniaFact]
    public void Adding_An_Item_Grows_The_Realized_Containers()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("ItemList")!;

        vm.Items.Add("Gamma");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(3, itemsControl.GetRealizedContainers().Count());
        Assert.Contains("Gamma", Texts(itemsControl));
    }

    [AvaloniaFact]
    public void Removing_An_Item_Shrinks_The_Realized_Containers()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("ItemList")!;

        vm.Items.Add("Gamma");
        Dispatcher.UIThread.RunJobs();

        vm.Items.Remove("Alpha");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(2, itemsControl.GetRealizedContainers().Count());
        var texts = Texts(itemsControl);
        Assert.DoesNotContain("Alpha", texts);
        Assert.Contains("Beta", texts);
        Assert.Contains("Gamma", texts);
    }
}
