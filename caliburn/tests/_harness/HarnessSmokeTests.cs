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

    // Proves the ViewLocator.NameTransformer reset added for ex015. Two separate facts,
    // deliberately not one: this is the same "reset at the start of every test" shape as the
    // other three globals, so the proof has to be that ONE test's mutation cannot survive into
    // ANOTHER test, not just that a single test can observe 4 in isolation. Both facts mutate
    // on purpose: xunit.v3 orders test cases within a class by a stable sort over case IDs, NOT
    // source order, so if only one fact mutated, a broken reset would pass or fail depending on
    // which of the two happened to sort first -- proving nothing reliably. With both mutating,
    // whichever runs second sees 5 (not 4) the moment the reset is missing, regardless of order.
    [Fact]
    public void First_Test_Sees_The_Pristine_Count_Then_Mutates_It()
    {
        // If this ever sees 5 here, either the reset broke or a previous test's rule leaked in.
        Assert.Equal(4, ViewLocator.NameTransformer.Count);

        ViewLocator.NameTransformer.AddRule("HarnessSmokeIgnoredA$", "Ignored");

        Assert.Equal(5, ViewLocator.NameTransformer.Count);
    }

    [Fact]
    public void Second_Test_Also_Sees_The_Pristine_Count_Then_Mutates_It()
    {
        // Without the harness reset, this fails whenever it happens to run after the test
        // above -- proving the mutation really would otherwise leak across tests, regardless
        // of which of these two facts xunit decides to run first.
        Assert.Equal(4, ViewLocator.NameTransformer.Count);

        ViewLocator.NameTransformer.AddRule("HarnessSmokeIgnoredB$", "Ignored");

        Assert.Equal(5, ViewLocator.NameTransformer.Count);
    }

    // Proves the LogManager.GetLog reset added for ex063. Same shape as the NameTransformer pair
    // above, for the same reason: xunit.v3 orders test cases within a class by a stable sort over
    // case IDs, not source order, so a single mutating fact would prove nothing about order
    // independence - both facts have to mutate, so whichever runs second observes the leak if the
    // reset is ever removed. Unlike NameTransformer, GetLog has no count to compare - it is a
    // single delegate value - so "pristine" is proven by reference identity against
    // CaliburnCoreContext's own protected PristineGetLog snapshot instead.
    [Fact]
    public void First_Test_Sees_The_Pristine_GetLog_Delegate_Then_Replaces_It()
    {
        // If the OTHER fact's replacement below ever leaked into this one, GetLog would already
        // be some OTHER delegate here, not the same reference captured once in the static ctor.
        Assert.Same(PristineGetLog, LogManager.GetLog);

        LogManager.GetLog = _ => new HarnessNoOpLog();

        Assert.NotSame(PristineGetLog, LogManager.GetLog);
    }

    [Fact]
    public void Second_Test_Also_Sees_The_Pristine_GetLog_Delegate_Then_Replaces_It()
    {
        // Without the harness reset, this fails whenever it happens to run after the test above
        // -- GetLog would still be the OTHER fact's replacement instead of freshly reset back to
        // PristineGetLog, regardless of which of these two facts xunit decides to run first.
        Assert.Same(PristineGetLog, LogManager.GetLog);

        LogManager.GetLog = _ => new HarnessNoOpLog();

        Assert.NotSame(PristineGetLog, LogManager.GetLog);
    }

    /// <summary>A do-nothing ILog, only ever needed as a distinct delegate target above - never
    /// actually invoked, so its three methods are bodies-only, not a recording double.</summary>
    sealed class HarnessNoOpLog : ILog
    {
        public void Info(string format, params object[] args) { }
        public void Warn(string format, params object[] args) { }
        public void Error(Exception exception) { }
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

    // Proves ShowDialogAndCloseAsync (added for ex046-ex050) before any catalog exercise
    // depends on it: WindowManager.ShowDialogAsync is modal, so this is the one place the
    // recipe needs to be right - a wrong recipe here would otherwise surface, confusingly, as
    // a hang in whichever exercise happened to use it first.
    [WpfFact]
    public async Task ShowDialogAndCloseAsync_Resolves_Without_Hanging_The_Nested_Modal_Loop()
    {
        var vm = new Screen();

        var (result, window) = await ShowDialogAndCloseAsync(vm, true);

        Assert.True(result);
        Assert.NotNull(window);
    }
}
