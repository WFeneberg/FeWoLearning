using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex024_ActionGuardRefreshTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <Button x:Name="Guarded" Content="Guarded" />
            <Button x:Name="ByMethod" Content="ByMethod" />
          </StackPanel>
        </UserControl>
        """;

    (Ex024_Vm Vm, Button Guarded, Button ByMethod) Bound(Ex024_Vm vm)
    {
        var subject = new Ex024_ActionGuardRefresh();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(vm, view);
        Show(view);
        return (vm, (Button)view.FindName("Guarded")!, (Button)view.FindName("ByMethod")!);
    }

    [WpfFact]
    public void A_Silent_Property_Guard_Change_Leaves_IsEnabled_Stale_Until_Explicitly_Announced()
    {
        var (vm, guarded, _) = Bound(new Ex024_Vm(canGuarded: false));
        Assert.False(guarded.IsEnabled);

        // Move the guard, but WITHOUT going through the notifying setter.
        vm.SetGuardSilently(true);
        Pump();

        // A wrong implementation that (incorrectly) reads CanGuarded fresh on every layout
        // pass, instead of only on a real PropertyChanged, would pass every other test in
        // this class and fail only here.
        Assert.False(guarded.IsEnabled);

        // Now announce it - the fix.
        vm.AnnounceGuard();
        Pump();

        Assert.True(guarded.IsEnabled);
    }

    [WpfFact]
    public void A_Method_Guard_Evaluates_Correctly_At_Bind_Time_When_It_Returns_False()
    {
        var (_, _, byMethod) = Bound(new Ex024_Vm(canByMethod: false));

        Assert.False(byMethod.IsEnabled);
    }

    [WpfFact]
    public void A_Method_Guard_Evaluates_Correctly_At_Bind_Time_When_It_Returns_True()
    {
        var (_, _, byMethod) = Bound(new Ex024_Vm(canByMethod: true));

        // Proves the initial read genuinely calls CanByMethod() - a wrong implementation that
        // just always started every guarded button disabled would pass the False-at-bind-time
        // test above and fail only here.
        Assert.True(byMethod.IsEnabled);
    }

    [WpfFact]
    public void A_Method_Guard_Stays_Stale_After_A_Targeted_Announce_Naming_The_Method()
    {
        var (vm, _, byMethod) = Bound(new Ex024_Vm(canByMethod: false));
        Assert.False(byMethod.IsEnabled);

        vm.SetByMethodSilently(true);
        vm.AnnounceByMethodGuard();
        Pump();

        // There is no property named CanByMethod for the notification to match - the targeted
        // announce that fixed the property guard above does nothing for a method guard.
        Assert.False(byMethod.IsEnabled);
    }

    [WpfFact]
    public void A_Method_Guard_Stays_Stale_Even_After_A_Full_Refresh()
    {
        var (vm, _, byMethod) = Bound(new Ex024_Vm(canByMethod: false));
        Assert.False(byMethod.IsEnabled);

        vm.SetByMethodSilently(true);
        vm.Refresh();
        Pump();

        Assert.False(byMethod.IsEnabled);
    }
}
