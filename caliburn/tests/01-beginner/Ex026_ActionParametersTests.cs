using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using Microsoft.Xaml.Behaviors;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex026_ActionParametersTests : CaliburnViewContext
{
    static (Ex026_Vm Vm, FrameworkElement View, TextBox Box, Button FromElementButton, Button CoercedButton) Built()
    {
        var subject = new Ex026_ActionParameters();
        var vm = new Ex026_Vm();
        var (view, box, fromElementButton, coercedButton) = subject.BuildView(vm);
        return (vm, view, box, fromElementButton, coercedButton);
    }

    [WpfFact]
    public void FromElementButton_Carries_A_Real_ActionMessage_Trigger_Named_FromElement()
    {
        var (_, _, _, fromElementButton, _) = Built();

        var trigger = Interaction.GetTriggers(fromElementButton)
            .OfType<Microsoft.Xaml.Behaviors.EventTrigger>()
            .Single(t => t.EventName == "Click");
        var action = Assert.IsType<ActionMessage>(trigger.Actions.Single());

        Assert.Equal("FromElement", action.MethodName);
    }

    [WpfFact]
    public void CoercedButton_Carries_A_Real_ActionMessage_Trigger_Named_Coerced()
    {
        var (_, _, _, _, coercedButton) = Built();

        var trigger = Interaction.GetTriggers(coercedButton)
            .OfType<Microsoft.Xaml.Behaviors.EventTrigger>()
            .Single(t => t.EventName == "Click");
        var action = Assert.IsType<ActionMessage>(trigger.Actions.Single());

        Assert.Equal("Coerced", action.MethodName);
    }

    [WpfFact]
    public void Clicking_FromElement_Receives_The_Boxs_Current_Text_As_A_String()
    {
        var (vm, view, box, fromElementButton, _) = Built();
        Show(view);
        // Changed AFTER BuildView ran - proves this is a live read of the element at click time,
        // not a value captured once while the attach string was being parsed.
        box.Text = "99";

        fromElementButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vm.FromElementCallCount);
        Assert.Equal("99", vm.FromElementValue);
    }

    [WpfFact]
    public void Clicking_Coerced_Receives_The_Boxs_Text_Converted_To_A_Real_Int32()
    {
        var (vm, view, box, _, coercedButton) = Built();
        Show(view);
        box.Text = "99";

        coercedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vm.CoercedCallCount);
        Assert.Equal(99, vm.CoercedValue);
    }

    [WpfFact]
    public void Clicking_Without_Touching_Box_Receives_The_Documented_Initial_Value_42()
    {
        // Neither click test above ever observes the view's INITIAL Text - both overwrite it
        // with "99" first. This one never touches Box.Text at all, so a solution that omitted
        // Text="42" from the XAML entirely would still go green everywhere else and only fail
        // here.
        var (vm, view, _, fromElementButton, coercedButton) = Built();
        Show(view);

        fromElementButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();
        coercedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal("42", vm.FromElementValue);
        Assert.Equal(42, vm.CoercedValue);
    }

    [WpfFact]
    public void Clicking_FromElement_Twice_Invokes_Twice_Not_Once()
    {
        var (vm, view, _, fromElementButton, _) = Built();
        Show(view);

        fromElementButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();
        fromElementButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(2, vm.FromElementCallCount);
    }

    [WpfFact]
    public void Without_Hosting_In_A_Real_Window_Clicking_Invokes_Nothing()
    {
        var (vm, view, _, fromElementButton, _) = Built();

        Layout(view);
        fromElementButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(0, vm.FromElementCallCount);
    }
}
