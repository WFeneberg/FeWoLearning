using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Caliburn.Micro;
using Microsoft.Xaml.Behaviors;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex022_ActionConventionButtonTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <Button x:Name="Plain" Content="Plain" />
            <Button x:Name="Unmatched" Content="Unmatched" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex022_Vm Vm, FrameworkElement View, Button Plain, Button Unmatched) Bound()
    {
        var subject = new Ex022_ActionConventionButton();
        var vm = new Ex022_Vm();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(vm, view);
        var plain = (Button)view.FindName("Plain")!;
        var unmatched = (Button)view.FindName("Unmatched")!;
        return (vm, view, plain, unmatched);
    }

    [WpfFact]
    public void Binding_Attaches_A_Real_ActionMessage_Trigger_Named_After_The_Method()
    {
        var (_, _, plain, _) = Bound();

        var trigger = Interaction.GetTriggers(plain)
            .OfType<Microsoft.Xaml.Behaviors.EventTrigger>()
            .Single(t => t.EventName == "Click");
        var action = Assert.IsType<ActionMessage>(trigger.Actions.Single());

        Assert.Equal("Plain", action.MethodName);
    }

    [WpfFact]
    public void A_Button_Whose_Name_Matches_No_Method_Gets_No_Trigger_At_All()
    {
        var (_, _, _, unmatched) = Bound();

        Assert.Empty(Interaction.GetTriggers(unmatched));
    }

    [WpfFact]
    public void Raising_Click_On_The_Hosted_View_Invokes_The_Method_Once()
    {
        var (vm, view, plain, _) = Bound();
        Show(view);

        plain.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vm.ClickCount);
    }

    [WpfFact]
    public void Raising_Click_Twice_Invokes_The_Method_Twice_Not_Once()
    {
        var (vm, view, plain, _) = Bound();
        Show(view);

        plain.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();
        plain.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        // A wrong implementation that wires the click to some fixed "already ran" flag,
        // instead of really invoking Plain() through the action message each time, would
        // pass the single-click test above and fail only here.
        Assert.Equal(2, vm.ClickCount);
    }

    [WpfFact]
    public void Without_Hosting_In_A_Real_Window_Raising_Click_Invokes_Nothing()
    {
        var (vm, view, plain, _) = Bound();

        // Measure/Arrange only - the trigger exists (see the structural test above) but has
        // no PresentationSource to resolve its source through, so the click is a no-op.
        Layout(view);
        plain.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(0, vm.ClickCount);
    }
}
