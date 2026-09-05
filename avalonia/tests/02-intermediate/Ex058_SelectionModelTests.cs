using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex058_SelectionModelTests
{
    private static (Ex058_SelectionModel View, Ex058_SelectionModelViewModel Vm) Arrange()
    {
        var vm = new Ex058_SelectionModelViewModel();
        var view = ViewHarness.Show(new Ex058_SelectionModel { DataContext = vm }, 200, 150);
        return (view, vm);
    }

    // Mechanism check: List.Selection must be bound to the SAME SelectionModel
    // instance the view model exposes - this is what distinguishes "the ListBox
    // and the view model share one model" from "the ListBox merely allows
    // multiple selection on its own". Measured on this machine: binding
    // Selection="{Binding Selection}" makes the two references identical.
    [AvaloniaFact]
    public void List_Shares_The_ViewModels_SelectionModel_Instance()
    {
        var (view, vm) = Arrange();
        var list = view.FindControl<ListBox>("List");

        Assert.NotNull(list);
        Assert.Same(vm.Items, list!.ItemsSource);
        Assert.Same(vm.Selection, list.Selection);
        Assert.False(vm.Selection.SingleSelect);
    }

    // The real discriminator: selecting through the ListBox's own Selection
    // must be visible through the view model's Selection object - a design
    // that lets the ListBox select multiple items on its own (e.g. via
    // SelectionMode="Multiple" and SelectedItems, with no Selection binding at
    // all) can satisfy "multi-select works in the UI" while leaving the view
    // model's own model at Count == 0 regardless of what gets selected. This
    // bit ex024, which asserted ListBox.SelectedItems and never noticed.
    [AvaloniaFact]
    public void Selecting_Two_Items_Through_The_Shared_Model_Is_Visible_On_The_ViewModel()
    {
        var (view, vm) = Arrange();
        var list = view.FindControl<ListBox>("List")!;
        Dispatcher.UIThread.RunJobs();

        list.Selection.Select(0);
        list.Selection.Select(2);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(2, vm.Selection.Count);
        Assert.Contains(0, vm.Selection.SelectedIndexes);
        Assert.Contains(2, vm.Selection.SelectedIndexes);
        Assert.Contains("red", vm.Selection.SelectedItems);
        Assert.Contains("blue", vm.Selection.SelectedItems);
        Assert.DoesNotContain("green", vm.Selection.SelectedItems);
    }
}
