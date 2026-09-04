using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex024_ListBoxSelectionTests
{
    private static (Ex024_ListBoxSelection View, Ex024_ListBoxSelectionViewModel Vm) Arrange()
    {
        var vm = new Ex024_ListBoxSelectionViewModel();
        var view = ViewHarness.Show(new Ex024_ListBoxSelection { DataContext = vm }, 300, 150);
        return (view, vm);
    }

    // Mechanism check: a hard-coded set of ListBoxItem children instead of a
    // bound ItemsSource can render identically but ItemsSource stays null.
    [AvaloniaFact]
    public void ItemsSource_Is_The_Vms_Items_Collection()
    {
        var (view, vm) = Arrange();
        var list = view.FindControl<ListBox>("ColorsList")!;

        Assert.Same(vm.Items, list.ItemsSource);
    }

    // Mechanism check: SelectionMode is what the exercise is actually about,
    // and the default is Single - assert the property itself, not only its
    // effect (the effect is also checked below, independently).
    [AvaloniaFact]
    public void Uses_Multiple_SelectionMode()
    {
        var (view, _) = Arrange();
        var list = view.FindControl<ListBox>("ColorsList")!;

        Assert.Equal(SelectionMode.Multiple, list.SelectionMode);
    }

    [AvaloniaFact]
    public void SelectedIndex_Selects_The_Item_At_That_Position()
    {
        var (view, _) = Arrange();
        var list = view.FindControl<ListBox>("ColorsList")!;

        list.SelectedIndex = 2;
        Assert.Equal("blue", list.SelectedItem);

        list.SelectedIndex = 0;
        Assert.Equal("red", list.SelectedItem);
    }

    // The real proof of Multiple: a single-select ListBox cannot hold two
    // entries in SelectedItems at once.
    [AvaloniaFact]
    public void SelectedItems_Collects_Multiple_Selections_Under_Multiple_Mode()
    {
        var (view, _) = Arrange();
        var list = view.FindControl<ListBox>("ColorsList")!;

        list.SelectedItems!.Add("red");
        list.SelectedItems.Add("blue");

        Assert.Equal(2, list.SelectedItems.Count);
        var selected = list.SelectedItems.Cast<string>().ToList();
        Assert.Contains("red", selected);
        Assert.Contains("blue", selected);
    }
}
