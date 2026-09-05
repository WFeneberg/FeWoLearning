using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using Microsoft.Xaml.Behaviors;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex027_ActionSpecialValuesTests : CaliburnViewContext
{
    static (Ex027_Vm Vm, FrameworkElement View, Button Button) Built()
    {
        var subject = new Ex027_ActionSpecialValues();
        var vm = new Ex027_Vm();
        var (view, button) = subject.BuildView(vm);
        return (vm, view, button);
    }

    [WpfFact]
    public void Button_Carries_A_Real_ActionMessage_Trigger_Named_CaptureAll()
    {
        var (_, _, button) = Built();

        var trigger = Interaction.GetTriggers(button)
            .OfType<Microsoft.Xaml.Behaviors.EventTrigger>()
            .Single(t => t.EventName == "Click");
        var action = Assert.IsType<ActionMessage>(trigger.Actions.Single());

        Assert.Equal("CaptureAll", action.MethodName);
    }

    [WpfFact]
    public void Clicking_Passes_The_Very_RoutedEventArgs_As_EventArgs()
    {
        var (vm, view, button) = Built();
        Show(view);
        var clickArgs = new RoutedEventArgs(ButtonBase.ClickEvent);

        button.RaiseEvent(clickArgs);
        Pump();

        Assert.Equal(1, vm.CallCount);
        Assert.Same(clickArgs, vm.LastEventArgs);
    }

    [WpfFact]
    public void Clicking_Passes_The_View_Models_Own_Instance_As_DataContext()
    {
        var (vm, view, button) = Built();
        Show(view);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Same(vm, vm.LastDataContext);
    }

    [WpfFact]
    public void Clicking_Passes_The_Clicked_Button_Itself_As_Source()
    {
        var (vm, view, button) = Built();
        Show(view);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Same(button, vm.LastSource);
    }

    [WpfFact]
    public void Clicking_Passes_The_Views_Root_As_View_A_Different_Object_From_Source()
    {
        var (vm, view, button) = Built();
        Show(view);

        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Same(view, vm.LastView);
        // The sharp part this exercise measures: $view is NOT just another name for $source.
        Assert.NotSame(vm.LastSource, vm.LastView);
    }

    [WpfFact]
    public void Clicking_Passes_The_View_Model_As_This_Even_Though_It_Is_Not_A_SpecialValues_Key()
    {
        var (vm, view, button) = Built();

        Assert.False(MessageBinder.SpecialValues.ContainsKey("$this"));

        Show(view);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Same(vm, vm.LastThis);
    }

    [WpfFact]
    public void Without_Hosting_In_A_Real_Window_Clicking_Invokes_Nothing()
    {
        var (vm, view, button) = Built();

        Layout(view);
        button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(0, vm.CallCount);
    }
}
