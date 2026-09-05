using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex013_TwoWayUpdateSourceTriggerTests : WpfTestContext
{
    [WpfFact]
    public void Editing_The_Target_Leaves_The_Source_Alone_Until_Commit_Is_Called()
    {
        var source = new Ex013_DraftSource { Label = "inlet" };
        var target = new TextBox();
        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);
        Layout(target);

        target.Text = "edited";
        Pump();

        // Negative: UpdateSourceTrigger.Explicit means nothing pushes to the source
        // on its own - a learner who left the default PropertyChanged trigger (or
        // Mode.OneWay, which never pushes at all) could each pass a test that stopped
        // here, so it is paired below with a positive assertion on the very same
        // target/source instances. This alone cannot tell Explicit apart from an
        // unset trigger, though - see the focus-based test below for that.
        Assert.Equal("inlet", source.Label);

        Ex013_TwoWayUpdateSourceTrigger.Commit(target);

        // Positive, same instances: proves the binding was live and two-way, simply
        // waiting for the explicit push - not disconnected, not OneWay, not silently
        // dropped.
        Assert.Equal("edited", source.Label);
    }

    [WpfFact]
    public void Losing_Focus_Does_Not_Push_The_Way_The_Unset_Default_Trigger_Would()
    {
        var source = new Ex013_DraftSource { Label = "inlet" };
        var target = new TextBox();
        var elsewhere = new TextBox();
        var scope = new Grid();
        FocusManager.SetIsFocusScope(scope, true);
        scope.Children.Add(target);
        scope.Children.Add(elsewhere);

        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);
        Layout(scope);

        FocusManager.SetFocusedElement(scope, target);
        Pump();

        target.Text = "edited";
        Pump();

        // Moves logical focus off target, which raises its LostFocus - the trigger a
        // learner who left UpdateSourceTrigger unset (TextBox.Text's own default)
        // would be relying on. No Show(...) and no synthetic input needed: logical
        // focus works on a windowless tree.
        FocusManager.SetFocusedElement(scope, elsewhere);
        Pump();

        // Explicit ignores LostFocus entirely - the edit is still only pending.
        Assert.Equal("inlet", source.Label);
    }

    [WpfFact]
    public void Source_Changes_Still_Reach_The_Target_Immediately()
    {
        var source = new Ex013_DraftSource { Label = "inlet" };
        var target = new TextBox();
        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);
        Layout(target);

        source.Label = "outlet";
        Pump();

        // UpdateSourceTrigger only governs the target -> source direction. Source ->
        // target still runs on PropertyChanged, same as ex004, with no explicit call
        // needed.
        Assert.Equal("outlet", target.Text);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_The_Expected_Path_Mode_And_Explicit_Trigger()
    {
        var source = new Ex013_DraftSource();
        var target = new TextBox();

        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);

        var binding = BindingOperations.GetBinding(target, TextBox.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex013_DraftSource.Label), binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.Explicit, binding.UpdateSourceTrigger);
    }

    [WpfFact]
    public void The_Source_Is_Reached_Through_The_DataContext()
    {
        var source = new Ex013_DraftSource();
        var target = new TextBox();

        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);

        Assert.Same(source, target.DataContext);
    }
}
