using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex025_ItemsControlTemplateTests
{
    private static (Ex025_ItemsControlTemplate View, Ex025_ItemsControlTemplateViewModel Vm) Arrange()
    {
        var vm = new Ex025_ItemsControlTemplateViewModel();
        var view = ViewHarness.Show(new Ex025_ItemsControlTemplate { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    // Mechanism check: a control named Fruits that is not actually an
    // ItemsControl (e.g. a hard-coded StackPanel given the same name) fails
    // this typed lookup outright.
    [AvaloniaFact]
    public void Fruits_Is_An_ItemsControl_Bound_To_The_Vms_Items()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Fruits");

        Assert.NotNull(itemsControl);
        Assert.Same(vm.Items, itemsControl!.ItemsSource);
        Assert.NotNull(itemsControl.ItemTemplate);
    }

    [AvaloniaFact]
    public void Realizes_One_Container_Per_Item_With_The_Right_Text()
    {
        var (view, _) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Fruits")!;

        Assert.Equal(3, itemsControl.GetRealizedContainers().Count());

        var texts = itemsControl.GetVisualDescendants().OfType<TextBlock>()
            .Select(t => t.Text)
            .ToList();
        Assert.Contains("Apple", texts);
        Assert.Contains("Banana", texts);
        Assert.Contains("Cherry", texts);
    }

    // The real discriminator against a hard-coded panel of TextBlocks: the
    // rendered list must react to a collection change on the view model, not
    // just show the right thing once at startup.
    [AvaloniaFact]
    public void Reacts_To_A_Collection_Change()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Fruits")!;

        vm.Items.Add("Date");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(4, itemsControl.GetRealizedContainers().Count());
        var texts = itemsControl.GetVisualDescendants().OfType<TextBlock>()
            .Select(t => t.Text)
            .ToList();
        Assert.Contains("Date", texts);
    }
}
