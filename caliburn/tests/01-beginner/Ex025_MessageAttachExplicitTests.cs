using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Caliburn.Micro;
using Microsoft.Xaml.Behaviors;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex025_MessageAttachExplicitTests : CaliburnViewContext
{
    // NotAMethodName is deliberate: nothing about this Button's x:Name matches any method on
    // Ex025_Vm - the whole point of cal:Message.Attach is that it does not need to.
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform">
          <StackPanel>
            <Button x:Name="NotAMethodName" Content="Go" cal:Message.Attach="WithParam('abcd')" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex025_Vm Vm, FrameworkElement View, Button Button) Attached()
    {
        var subject = new Ex025_MessageAttachExplicit();
        var vm = new Ex025_Vm();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.AttachViewModel(view, vm);
        return (vm, view, (Button)view.FindName("NotAMethodName")!);
    }

    [WpfFact]
    public void AttachViewModel_Sets_The_Views_DataContext_To_The_View_Model()
    {
        var (vm, view, _) = Attached();

        Assert.Same(vm, view.DataContext);
    }

    [WpfFact]
    public void The_Explicit_Attach_Wires_A_Real_ActionMessage_Trigger_Naming_WithParam()
    {
        var (_, _, button) = Attached();

        var trigger = Interaction.GetTriggers(button)
            .OfType<Microsoft.Xaml.Behaviors.EventTrigger>()
            .Single(t => t.EventName == "Click");
        var action = Assert.IsType<ActionMessage>(trigger.Actions.Single());

        // The trigger names WithParam, not NotAMethodName - proof the wiring came from the
        // explicit attach string, not from the button happening to match a method by name.
        Assert.Equal("WithParam", action.MethodName);
    }

    [WpfFact]
    public void Clicking_The_Hosted_Button_Invokes_WithParam_With_The_Literal_String_Parameter()
    {
        var (vm, view, button) = Attached();
        Show(view);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vm.Count);
        Assert.Equal("abcd", vm.LastParam);
    }

    [WpfFact]
    public void Clicking_Twice_Invokes_Twice_With_The_Same_Literal_Parameter_Each_Time()
    {
        var (vm, view, button) = Attached();
        Show(view);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(2, vm.Count);
        Assert.Equal("abcd", vm.LastParam);
    }

    [WpfFact]
    public void Without_Hosting_In_A_Real_Window_Clicking_Invokes_Nothing()
    {
        var (vm, view, button) = Attached();

        Layout(view);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(0, vm.Count);
    }
}
