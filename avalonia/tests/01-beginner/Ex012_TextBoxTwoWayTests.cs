using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex012_TextBoxTwoWayTests
{
    private static (Ex012_TextBoxTwoWay View, Ex012_TextBoxTwoWayViewModel Vm) Arrange()
    {
        var vm = new Ex012_TextBoxTwoWayViewModel();
        var view = ViewHarness.Show(new Ex012_TextBoxTwoWay { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_The_Vms_Starting_Value()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<TextBox>("MessageBox")!;

        Assert.Equal(vm.Message, box.Text);
    }

    // The discriminator Ex011's TwoWay case does not cover. There, the test sets
    // TextBox.Text via code, one whole-string assignment at a time - which any
    // TwoWay binding satisfies trivially. Here real key input is simulated one
    // character at a time, through the actual input pipeline, and the view model
    // is asserted current after EACH keystroke - never only at the end, and
    // never after any focus loss. Avalonia's TextBox.Text has no
    // UpdateSourceTrigger concept and no LostFocus mode the way WPF's does: a
    // TwoWay binding pushes on every keystroke while the box is still focused.
    [AvaloniaFact]
    public void Updates_The_Vm_After_Every_Keystroke_Without_Losing_Focus()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<TextBox>("MessageBox")!;
        box.Text = "";
        box.Focus();
        Dispatcher.UIThread.RunJobs();

        var topLevel = TopLevel.GetTopLevel(view)!;

        topLevel.KeyTextInput("H");
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("H", vm.Message);
        Assert.True(box.IsFocused);

        topLevel.KeyTextInput("i");
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Hi", vm.Message);
        Assert.True(box.IsFocused);

        topLevel.KeyTextInput("!");
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Hi!", vm.Message);
        Assert.True(box.IsFocused);
    }

    [AvaloniaFact]
    public void Vm_Change_Still_Flows_To_The_Box()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<TextBox>("MessageBox")!;

        vm.Message = "from-vm";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("from-vm", box.Text);
    }
}
