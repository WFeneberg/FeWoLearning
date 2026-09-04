using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex019_ButtonClickEventTests
{
    private static (Ex019_ButtonClickEvent View, Ex019_ButtonClickEventViewModel Vm) Arrange()
    {
        var vm = new Ex019_ButtonClickEventViewModel();
        var view = ViewHarness.Show(new Ex019_ButtonClickEvent { DataContext = vm }, 300, 160);
        return (view, vm);
    }

    private static void Click(Visual target)
    {
        var top = TopLevel.GetTopLevel(target)!;
        var p = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), top)!.Value;
        top.MouseDown(p, MouseButton.Left);
        top.MouseUp(p, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    // This is the actual contrast the exercise is about, and what makes it NOT
    // a repeat of Ex016: one button is driven purely by a Click event (so it
    // must have no Command at all), the other purely by a ReactiveCommand.
    [AvaloniaFact]
    public void The_Event_Button_Has_No_Command_While_The_Command_Button_Is_Bound_To_The_Vm()
    {
        var (view, vm) = Arrange();
        var eventButton = view.FindControl<Button>("EventButton")!;
        var commandButton = view.FindControl<Button>("CommandButton")!;

        Assert.Null(eventButton.Command);
        Assert.Same(vm.CommandClickCommand, commandButton.Command);
    }

    [AvaloniaFact]
    public void Clicking_The_Event_Button_Only_Moves_The_Event_Counter()
    {
        var (view, vm) = Arrange();
        var eventButton = view.FindControl<Button>("EventButton")!;

        Click(eventButton);
        Assert.Equal(1, vm.EventClickCount);
        Assert.Equal(0, vm.CommandClickCount);

        Click(eventButton);
        Assert.Equal(2, vm.EventClickCount);
        Assert.Equal(0, vm.CommandClickCount);
    }

    [AvaloniaFact]
    public void Clicking_The_Command_Button_Only_Moves_The_Command_Counter()
    {
        var (view, vm) = Arrange();
        var commandButton = view.FindControl<Button>("CommandButton")!;

        Click(commandButton);
        Assert.Equal(0, vm.EventClickCount);
        Assert.Equal(1, vm.CommandClickCount);

        Click(commandButton);
        Assert.Equal(0, vm.EventClickCount);
        Assert.Equal(2, vm.CommandClickCount);
    }
}
