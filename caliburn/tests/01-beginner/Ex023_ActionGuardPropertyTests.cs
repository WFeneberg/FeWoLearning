using System.Windows;
using System.Windows.Controls;
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

    (Ex023_ActionGuardProperty Subject, Ex023_Vm Vm, FrameworkElement View, Button Guarded, Button Unguarded) BoundNotLoaded(Ex023_Vm vm)
    {
        var subject = new Ex023_ActionGuardProperty();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(vm, view);
        return (subject, vm, view, (Button)view.FindName("Guarded")!, (Button)view.FindName("Unguarded")!);
    }

    (Ex023_ActionGuardProperty Subject, Ex023_Vm Vm, Button Guarded, Button Unguarded) Bound(Ex023_Vm vm)
    {
        var (subject, _, view, guarded, unguarded) = BoundNotLoaded(vm);
        Show(view);
        return (subject, vm, guarded, unguarded);
    }

    [WpfFact]
    public void Binding_Alone_Does_Not_Yet_Gate_The_Button_Loading_Does()
    {
        var (_, _, view, guarded, _) = BoundNotLoaded(new Ex023_Vm(canGuarded: false));

        // Right after Bind() - no Layout, no Load, no Show - the guard has not been evaluated
        // yet: ActionMessage defers reading it and subscribing to PropertyChanged through
        // View.ExecuteOnLoad, so the button measures ENABLED even though CanGuarded is false.
        // A wrong implementation that (incorrectly) expects the guard to apply the instant
        // Bind returns would pass every other test in this class and fail only here.
        Assert.True(guarded.IsEnabled);

        // Loading the view is what makes the guard apply - and a real window is NOT required
        // for THIS (unlike invoking the action itself, which needs Show - see ex022).
        Load(view);

        Assert.False(guarded.IsEnabled);
    }

    [WpfFact]
    public void CanGuarded_False_Disables_The_Button_Once_Loaded()
    {
        var (_, _, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));

        // No click, no explicit refresh - the guard is honoured purely from the view loading.
        Assert.False(guarded.IsEnabled);
    }

    [WpfFact]
    public void CanGuarded_True_Leaves_The_Button_Enabled_Once_Loaded()
    {
        var (_, _, guarded, _) = Bound(new Ex023_Vm(canGuarded: true));

        Assert.True(guarded.IsEnabled);
    }

    [WpfFact]
    public void Changing_CanGuarded_Through_Its_Public_Setter_Enables_The_Button_After_Pump()
    {
        var (_, vm, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));
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
        var (subject, _, guarded, _) = Bound(new Ex023_Vm(canGuarded: false));

        // Anchor the "no Binding" claim to a DEMONSTRABLY WORKING guard - without this first
        // assertion, the second would pass for any implementation, working or not.
        Assert.False(guarded.IsEnabled);
        Assert.False(subject.HasBinding(guarded, UIElement.IsEnabledProperty));
    }

    [WpfFact]
    public void A_Button_With_No_Matching_CanXxx_Property_Stays_Enabled_And_Ungated()
    {
        var (subject, _, _, unguarded) = Bound(new Ex023_Vm(canGuarded: false));

        Assert.True(unguarded.IsEnabled);
        Assert.False(subject.HasBinding(unguarded, UIElement.IsEnabledProperty));
    }
}
