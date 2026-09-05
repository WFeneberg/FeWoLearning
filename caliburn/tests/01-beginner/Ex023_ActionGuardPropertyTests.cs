using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex023_ActionGuardPropertyTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <Button x:Name="Guarded" Content="Guarded" />
            <Button x:Name="Unguarded" Content="Unguarded" />
          </StackPanel>
        </UserControl>
        """;

    (Ex023_Vm Vm, Button Guarded, Button Unguarded) Bound(Ex023_Vm vm)
    {
        var subject = new Ex023_ActionGuardProperty();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(vm, view);
        Show(view);
        return (vm, (Button)view.FindName("Guarded")!, (Button)view.FindName("Unguarded")!);
    }

    [WpfFact]
    public void CanGuarded_False_At_Bind_Time_Disables_The_Button_Immediately()
    {
        var (_, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));

        // No click, no explicit refresh - the guard is honoured purely from binding the view.
        Assert.False(guarded.IsEnabled);
    }

    [WpfFact]
    public void CanGuarded_True_At_Bind_Time_Leaves_The_Button_Enabled_Immediately()
    {
        var (_, guarded, _) = Bound(new Ex023_Vm(canGuarded: true));

        Assert.True(guarded.IsEnabled);
    }

    [WpfFact]
    public void Changing_CanGuarded_Through_Its_Public_Setter_Enables_The_Button_After_Pump()
    {
        var (vm, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));
        Assert.False(guarded.IsEnabled);

        // Moving the guard: a real property setter backed by Set() announces the change on
        // its own - no explicit NotifyOfPropertyChange call needed here (that nuance is ex024).
        vm.CanGuarded = true;
        Pump();

        Assert.True(guarded.IsEnabled);
    }

    [WpfFact]
    public void The_Guarded_IsEnabled_Is_Not_Wired_Through_A_Real_Binding_Even_Though_The_Guard_Works()
    {
        var (_, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));

        // The gating demonstrably works (see the tests above) - but, unlike every other
        // convention this track has measured, it is not implemented as a WPF Binding. A test
        // that instead asserted a Binding existed here would be asserting something false.
        Assert.Null(BindingOperations.GetBinding(guarded, UIElement.IsEnabledProperty));
    }

    [WpfFact]
    public void A_Button_With_No_Matching_CanXxx_Property_Stays_Enabled_And_Ungated()
    {
        var (_, _, unguarded) = Bound(new Ex023_Vm(canGuarded: false));

        Assert.True(unguarded.IsEnabled);
        Assert.Null(BindingOperations.GetBinding(unguarded, UIElement.IsEnabledProperty));
    }
}
