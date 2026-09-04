using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Proves the harness itself, so it is green in the real tree from the first commit
/// instead of first exercised at ex012. Not an exercise; no catalog.md row.
/// </summary>
public class HarnessCoreSmokeTests : CaliburnCoreContext
{
    [Fact]
    public void Core_context_initializes_IoC()
    {
        // Coroutine.BeginExecute calls IoC.BuildUp; without initialization this throws
        // "IoC is not initialized" and every coroutine exercise fails for the wrong reason.
        Assert.NotNull(IoC.BuildUp);
        Assert.NotNull(IoC.GetInstance);
    }
}

/// <summary>
/// The view half. Every test here is [WpfFact]: CaliburnViewContext installs
/// XamlPlatformProvider, which captures the current thread's Dispatcher, so a plain
/// [Fact] in this class would bind it to a thread that never pumps.
/// </summary>
public class HarnessSmokeTests : CaliburnViewContext
{
    [WpfFact]
    public void Default_control_templates_resolve_without_an_application()
    {
        var button = new Button { Content = "Hello" };
        button.ApplyTemplate();
        Layout(button);

        Assert.True(button.DesiredSize.Width > 0, $"width was {button.DesiredSize.Width}");
        Assert.True(button.DesiredSize.Height > 0, $"height was {button.DesiredSize.Height}");
    }

    [WpfFact]
    public void ViewLocator_finds_the_view_by_convention()
    {
        var view = ViewLocator.LocateForModel(new HarnessProbeViewModel(), null, null);

        Assert.IsType<HarnessProbeView>(view);
    }

    [WpfFact]
    public void ViewModelBinder_binds_by_name_in_both_directions()
    {
        var vm = new HarnessProbeViewModel();
        var view = new HarnessProbeView { DataContext = vm };
        ViewModelBinder.Bind(vm, view, null);
        Show(view);

        var box = (TextBox)view.FindName("UserName")!;
        Assert.Equal("Ada", box.Text);

        vm.UserName = "Grace";
        Pump();
        Assert.Equal("Grace", box.Text);
    }

    [WpfFact]
    public void Show_makes_guards_gate_and_actions_fire()
    {
        var vm = new HarnessProbeViewModel();
        var view = new HarnessProbeView { DataContext = vm };
        ViewModelBinder.Bind(vm, view, null);
        Show(view);

        var button = (Button)view.FindName("SayHello")!;

        // "Ada".Length > 3 is false, so the guard must have disabled it.
        Assert.False(button.IsEnabled);

        vm.UserName = "Grace";
        Pump();
        Assert.True(button.IsEnabled);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
        Pump();
        Assert.Equal(1, vm.Greetings);
    }
}
