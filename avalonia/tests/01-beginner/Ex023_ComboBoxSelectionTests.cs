using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex023_ComboBoxSelectionTests
{
    private static (Ex023_ComboBoxSelection View, Ex023_ComboBoxSelectionViewModel Vm) Arrange()
    {
        var vm = new Ex023_ComboBoxSelectionViewModel();
        var view = ViewHarness.Show(new Ex023_ComboBoxSelection { DataContext = vm }, 300, 100);
        return (view, vm);
    }

    // Mechanism check: a ComboBox with its own hard-coded ComboBoxItem
    // children instead of a bound ItemsSource can render identically but
    // its ItemsSource stays null - assert the source directly, by reference.
    [AvaloniaFact]
    public void ItemsSource_Is_The_Vms_Options_Collection()
    {
        var (view, vm) = Arrange();
        var combo = view.FindControl<ComboBox>("OptionsBox")!;

        Assert.Same(vm.Options, combo.ItemsSource);
    }

    [AvaloniaFact]
    public void Renders_The_Vms_Starting_Selection()
    {
        var (view, _) = Arrange();
        var combo = view.FindControl<ComboBox>("OptionsBox")!;

        Assert.Equal("one", combo.SelectedItem);
        Assert.Equal(0, combo.SelectedIndex);
    }

    [AvaloniaFact]
    public void Vm_Changes_Flow_To_The_ComboBox()
    {
        var (view, vm) = Arrange();
        var combo = view.FindControl<ComboBox>("OptionsBox")!;

        vm.Selected = "two";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("two", combo.SelectedItem);
        Assert.Equal(1, combo.SelectedIndex);

        vm.Selected = "three";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("three", combo.SelectedItem);
        Assert.Equal(2, combo.SelectedIndex);
    }

    [AvaloniaFact]
    public void ComboBox_Side_Selection_Writes_Back_To_The_Vm()
    {
        var (view, vm) = Arrange();
        var combo = view.FindControl<ComboBox>("OptionsBox")!;

        combo.SelectedItem = "three";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("three", vm.Selected);
    }
}
