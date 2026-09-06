using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex061_AsyncGuardRefreshTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <Button x:Name="Save" Content="Save" />
          </StackPanel>
        </UserControl>
        """;

    (Ex061_Vm Vm, Button Save) Bound(Ex061_Vm vm)
    {
        var subject = new Ex061_AsyncGuardRefresh();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(vm, view);
        Show(view);
        return (vm, (Button)view.FindName("Save")!);
    }

    [WpfFact]
    public void Save_Starts_Disabled_Before_Any_Refresh_Has_Ever_Run()
    {
        var (_, save) = Bound(new Ex061_Vm());

        Assert.False(save.IsEnabled);
    }

    [WpfFact]
    public async Task Save_Stays_Disabled_While_RefreshAsync_Is_Still_Awaiting_Its_Fetch()
    {
        var fetchGate = new TaskCompletionSource();
        var (vm, save) = Bound(new Ex061_Vm { FetchAsync = () => fetchGate.Task });

        var refreshTask = vm.RefreshAsync();
        Pump();

        // A stub that flips CanSave synchronously - ignoring FetchAsync entirely instead of
        // genuinely awaiting it - would already show Save enabled here, before the fetch has
        // ever completed.
        Assert.False(save.IsEnabled);

        fetchGate.SetResult();
        await BoundedAsync(refreshTask, "RefreshAsync to complete once its fetch resolves");
    }

    [WpfFact]
    public async Task Save_Becomes_Enabled_Once_RefreshAsync_Genuinely_Completes()
    {
        var fetchGate = new TaskCompletionSource();
        var (vm, save) = Bound(new Ex061_Vm { FetchAsync = () => fetchGate.Task });

        var refreshTask = vm.RefreshAsync();
        fetchGate.SetResult();
        await BoundedAsync(refreshTask, "RefreshAsync to complete");
        Pump();

        Assert.True(save.IsEnabled);
    }

    [WpfFact]
    public async Task Clicking_Save_After_Refresh_Completes_Actually_Invokes_It()
    {
        var (vm, save) = Bound(new Ex061_Vm { FetchAsync = () => Task.CompletedTask });

        await BoundedAsync(vm.RefreshAsync(), "RefreshAsync to complete");
        Pump();
        Assert.True(save.IsEnabled);

        save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, save));
        Pump();

        Assert.Equal(1, vm.SaveCount);
    }

    [WpfFact]
    public async Task RefreshAsync_With_No_FetchAsync_Supplied_Still_Completes_And_Enables_Save()
    {
        var (vm, save) = Bound(new Ex061_Vm());

        // A stub that dereferences FetchAsync unconditionally (instead of null-checking it
        // first) throws a NullReferenceException here rather than completing.
        await BoundedAsync(vm.RefreshAsync(), "RefreshAsync to complete with no fetch to await");
        Pump();

        Assert.True(save.IsEnabled);
    }
}
