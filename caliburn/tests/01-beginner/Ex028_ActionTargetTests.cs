using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;
// This test namespace nests inside FeWoLearning.Caliburn too, so a fully qualified
// Caliburn.Micro.Action reference resolves "Caliburn" against the ancestor namespace instead of
// the package root (CS0234) - see the matching comment in the Ex028 solution.
using CaliburnAction = Caliburn.Micro.Action;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex028_ActionTargetTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:cal="clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro.Platform">
          <StackPanel>
            <Button x:Name="ButtonA" Content="A" cal:Message.Attach="DoSomething" />
            <Button x:Name="ButtonB" Content="B" cal:Message.Attach="DoSomething" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex028_ActionTarget Subject, FrameworkElement View, Button ButtonA, Button ButtonB, Ex028_Vm VmA, Ex028_Vm VmB) Built()
    {
        var subject = new Ex028_ActionTarget();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var buttonA = (Button)view.FindName("ButtonA")!;
        var buttonB = (Button)view.FindName("ButtonB")!;
        return (subject, view, buttonA, buttonB, new Ex028_Vm(), new Ex028_Vm());
    }

    [WpfFact]
    public void SetTarget_Also_Sets_The_Elements_DataContext_To_The_Target()
    {
        var (subject, _, buttonA, _, vmA, _) = Built();

        subject.AttachWithContext(buttonA, vmA);

        Assert.Same(vmA, buttonA.DataContext);
    }

    [WpfFact]
    public void SetTargetWithoutContext_Leaves_The_Elements_DataContext_Untouched()
    {
        var (subject, _, _, buttonB, _, vmB) = Built();

        subject.AttachWithoutContext(buttonB, vmB);

        Assert.Null(buttonB.DataContext);
    }

    [WpfFact]
    public void Both_Forms_Report_HasTargetSet_True()
    {
        var (subject, _, buttonA, buttonB, vmA, vmB) = Built();

        subject.AttachWithContext(buttonA, vmA);
        subject.AttachWithoutContext(buttonB, vmB);

        Assert.True(CaliburnAction.HasTargetSet(buttonA));
        Assert.True(CaliburnAction.HasTargetSet(buttonB));
    }

    [WpfFact]
    public void Clicking_The_SetTarget_Button_Invokes_Its_Own_View_Models_Method()
    {
        var (subject, view, buttonA, _, vmA, _) = Built();
        subject.AttachWithContext(buttonA, vmA);
        Show(view);

        buttonA.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vmA.CallCount);
    }

    [WpfFact]
    public void Clicking_The_SetTargetWithoutContext_Button_Invokes_Its_Own_View_Models_Method_Despite_The_Null_DataContext()
    {
        var (subject, view, _, buttonB, _, vmB) = Built();
        subject.AttachWithoutContext(buttonB, vmB);
        Show(view);

        Assert.Null(buttonB.DataContext); // still untouched right up to the click
        buttonB.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(1, vmB.CallCount);
    }

    [WpfFact]
    public void Without_Hosting_In_A_Real_Window_Clicking_Invokes_Nothing()
    {
        var (subject, view, buttonA, _, vmA, _) = Built();
        subject.AttachWithContext(buttonA, vmA);

        Layout(view);
        buttonA.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Pump();

        Assert.Equal(0, vmA.CallCount);
    }
}
