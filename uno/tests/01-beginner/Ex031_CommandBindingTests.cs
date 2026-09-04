using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex031_CommandBindingTests : UnoTestContext
{
    private static Ex031_CommandBinding Command(Action<object?> execute, Func<bool> canExecute) =>
        new(execute, canExecute);

    [Fact]
    public void Answers_From_The_Predicate()
    {
        var allowed = false;
        var command = Command(_ => { }, () => allowed);

        Assert.False(command.CanExecute(null));

        allowed = true;
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Executes_When_Allowed()
    {
        var runs = 0;
        var command = Command(_ => runs++, () => true);

        command.Execute(null);

        Assert.Equal(1, runs);
    }

    [Fact]
    public void Refuses_To_Execute_When_Not_Allowed()
    {
        var runs = 0;
        var command = Command(_ => runs++, () => false);

        command.Execute(null);

        // Nothing in the framework guarantees a caller asked CanExecute first - a keyboard
        // accelerator or a test can call Execute directly.
        Assert.Equal(0, runs);
    }

    [Fact]
    public void Passes_The_Parameter_Through()
    {
        object? seen = null;
        var command = Command(p => seen = p, () => true);

        command.Execute("payload");

        Assert.Equal("payload", seen);
    }

    [Fact]
    public void Raising_The_Event_Notifies_Listeners()
    {
        var command = Command(_ => { }, () => true);
        var notifications = 0;
        command.CanExecuteChanged += (_, _) => notifications++;

        command.RaiseCanExecuteChanged();

        Assert.Equal(1, notifications);
    }

    [Fact]
    public void A_Bound_Button_Starts_Enabled_When_The_Command_Allows_It()
    {
        var button = Layout(Ex031_CommandBinding.CreateBoundButton(Command(_ => { }, () => true)));

        Assert.True(button.IsEnabled);
    }

    [Fact]
    public void A_Bound_Button_Starts_Disabled_When_The_Command_Refuses()
    {
        var button = Layout(Ex031_CommandBinding.CreateBoundButton(Command(_ => { }, () => false)));

        // Nobody set IsEnabled: the Command property did, by asking CanExecute.
        Assert.False(button.IsEnabled);
    }

    [Fact]
    public void A_Bound_Button_Re_Asks_When_Told_To()
    {
        var allowed = false;
        var command = Command(_ => { }, () => allowed);
        var button = Layout(Ex031_CommandBinding.CreateBoundButton(command));

        allowed = true;
        Assert.False(button.IsEnabled);

        command.RaiseCanExecuteChanged();

        // The predicate changing is invisible until the command says so - which is why a
        // view model has to raise this after anything that could change the answer.
        Assert.True(button.IsEnabled);
    }

    [Fact]
    public void Pressing_A_Bound_Button_Executes_The_Command()
    {
        var runs = 0;
        var button = Layout(Ex031_CommandBinding.CreateBoundButton(Command(_ => runs++, () => true)));

        new ButtonAutomationPeer(button).Invoke();

        Assert.Equal(1, runs);
    }

    [Fact]
    public void The_Button_Carries_The_Command()
    {
        var command = Command(_ => { }, () => true);

        var button = Ex031_CommandBinding.CreateBoundButton(command);

        Assert.Same(command, button.Command);
    }
}
