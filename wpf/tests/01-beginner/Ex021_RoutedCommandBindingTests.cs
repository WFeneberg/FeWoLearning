using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex021_RoutedCommandBindingTests : WpfTestContext
{
    [WpfFact]
    public void Wire_Registers_Exactly_One_CommandBinding_For_ApplicationCommands_Save()
    {
        var root = new StackPanel();

        Ex021_RoutedCommandBinding.Wire(root, (_, _) => { }, (_, e) => e.CanExecute = true);

        // Structural: proves Wire actually used owner.CommandBindings with the real
        // ApplicationCommands.Save instance, not some other collaborator.
        var binding = Assert.Single(root.CommandBindings.Cast<CommandBinding>());
        Assert.Same(ApplicationCommands.Save, binding.Command);
    }

    [WpfFact]
    public void Executed_Fires_When_An_Explicit_Target_Routes_Up_To_The_Wired_Ancestor()
    {
        var root = new StackPanel();
        var child = new StackPanel();
        var target = new Button();
        child.Children.Add(target);
        root.Children.Add(child);

        var executedCount = 0;
        Ex021_RoutedCommandBinding.Wire(root, (_, _) => executedCount++, (_, e) => e.CanExecute = true);
        Layout(root);

        // No focus, no Show(...): an explicit target routes up the tree on its own.
        ApplicationCommands.Save.Execute(null, target);
        Pump();

        Assert.Equal(1, executedCount);
    }

    [WpfFact]
    public void Execute_Does_Not_Fire_Executed_When_The_Wired_CanExecute_Returns_False()
    {
        var root = new StackPanel();
        var target = new Button();
        root.Children.Add(target);

        var executedCount = 0;
        Ex021_RoutedCommandBinding.Wire(root, (_, _) => executedCount++, (_, e) => e.CanExecute = false);
        Layout(root);

        // The behaviour that sets a RoutedCommand apart from a plain ICommand: Execute
        // itself consults CanExecute through the same CommandBinding first.
        ApplicationCommands.Save.Execute(null, target);
        Pump();

        Assert.Equal(0, executedCount);
    }

    [WpfFact]
    public void CanExecute_Reports_Exactly_What_The_Wired_Handler_Says()
    {
        var allow = new StackPanel();
        var allowTarget = new Button();
        allow.Children.Add(allowTarget);
        Ex021_RoutedCommandBinding.Wire(allow, (_, _) => { }, (_, e) => e.CanExecute = true);
        Layout(allow);

        var deny = new StackPanel();
        var denyTarget = new Button();
        deny.Children.Add(denyTarget);
        Ex021_RoutedCommandBinding.Wire(deny, (_, _) => { }, (_, e) => e.CanExecute = false);
        Layout(deny);

        Assert.True(ApplicationCommands.Save.CanExecute(null, allowTarget));
        Assert.False(ApplicationCommands.Save.CanExecute(null, denyTarget));
    }

    [WpfFact]
    public void A_CommandBinding_On_An_Unrelated_Element_Does_Not_Intercept()
    {
        var root = new StackPanel();
        var target = new Button();
        root.Children.Add(target);
        // No binding on root or any ancestor of target.
        Layout(root);

        var unrelated = new StackPanel();
        var executedCount = 0;
        Ex021_RoutedCommandBinding.Wire(unrelated, (_, _) => executedCount++, (_, e) => e.CanExecute = true);
        Layout(unrelated);

        // unrelated is not an ancestor of target, so routing must never reach it.
        ApplicationCommands.Save.Execute(null, target);
        Pump();

        Assert.Equal(0, executedCount);
    }
}
