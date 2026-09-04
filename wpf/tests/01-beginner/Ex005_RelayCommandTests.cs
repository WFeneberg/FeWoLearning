using System.Windows.Input;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex005_RelayCommandTests : WpfTestContext
{
    [WpfFact]
    public void Without_A_Predicate_The_Command_Can_Always_Execute()
    {
        var command = new Ex005_RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
        Assert.True(command.CanExecute("anything"));
    }

    [WpfFact]
    public void Execute_Invokes_The_Action_With_The_Parameter()
    {
        var seen = new List<object?>();
        var command = new Ex005_RelayCommand(seen.Add);

        command.Execute("payload");
        command.Execute(null);

        Assert.Equal(new object?[] { "payload", null }, seen);
    }

    [WpfFact]
    public void Can_Execute_Consults_The_Predicate_Every_Time()
    {
        var allowed = false;
        var command = new Ex005_RelayCommand(_ => { }, _ => allowed);

        Assert.False(command.CanExecute(null));

        allowed = true;

        // Re-asked, not cached: this is the whole reason WPF owns the polling.
        Assert.True(command.CanExecute(null));
    }

    [WpfFact]
    public void The_Predicate_Receives_The_Parameter()
    {
        var seen = new List<object?>();
        var command = new Ex005_RelayCommand(_ => { }, parameter =>
        {
            seen.Add(parameter);
            return true;
        });

        command.CanExecute(42);

        Assert.Equal(new object?[] { 42 }, seen);
    }

    [WpfFact]
    public void Can_Execute_Changed_Fires_When_The_Command_Manager_Requeries()
    {
        var command = new Ex005_RelayCommand(_ => { });
        var raised = 0;

        // Kept in a local on purpose: CommandManager.RequerySuggested stores its
        // handlers weakly, so an inline lambda with no strong reference anywhere can be
        // collected before the event is raised.
        EventHandler handler = (_, _) => raised++;
        command.CanExecuteChanged += handler;

        CommandManager.InvalidateRequerySuggested();
        Pump();

        // A hand-rolled event field would never see this - only forwarding to
        // RequerySuggested does.
        Assert.True(raised > 0, "CanExecuteChanged must be routed through CommandManager.RequerySuggested.");

        GC.KeepAlive(handler);
    }

    [WpfFact]
    public void Unsubscribing_Stops_The_Notifications()
    {
        var command = new Ex005_RelayCommand(_ => { });
        var raised = 0;
        EventHandler handler = (_, _) => raised++;

        // Prove the subscription works before proving the removal works. Asserting only
        // "nothing was raised after -=" would be satisfied by an implementation where
        // += never worked either, which is exactly the stub.
        command.CanExecuteChanged += handler;
        CommandManager.InvalidateRequerySuggested();
        Pump();
        var whileSubscribed = raised;
        Assert.True(whileSubscribed > 0, "The subscription must work before the removal can be tested.");

        command.CanExecuteChanged -= handler;
        CommandManager.InvalidateRequerySuggested();
        Pump();

        Assert.Equal(whileSubscribed, raised);
        GC.KeepAlive(handler);
    }

    [WpfFact]
    public void Is_Usable_Through_The_ICommand_Interface()
    {
        var executed = false;
        ICommand command = new Ex005_RelayCommand(_ => executed = true, _ => true);

        Assert.True(command.CanExecute(null));
        command.Execute(null);

        Assert.True(executed);
    }
}
