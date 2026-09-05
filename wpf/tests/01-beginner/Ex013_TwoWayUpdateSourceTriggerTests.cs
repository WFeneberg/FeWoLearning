using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex013_TwoWayUpdateSourceTriggerTests : WpfTestContext
{
    [WpfFact]
    public void Editing_The_Target_Leaves_The_Source_Alone_Until_UpdateSource_Is_Called()
    {
        var source = new Ex013_DraftSource { Label = "inlet" };
        var target = new TextBox();
        Ex013_TwoWayUpdateSourceTrigger.Bind(target, source);
        Layout(target);

        target.Text = "edited";
        Pump();

        // Negative: this is the load-bearing assertion. UpdateSourceTrigger.Explicit
        // means nothing pushes to the source on its own - a learner who left the
        // default PropertyChanged trigger (or Mode.OneWay, which never pushes at all)
        // could each pass a test that stopped here, so it is paired below with a
        // positive assertion on the very same target/source instances.
        Assert.Equal("inlet", source.Label);

        var expression = BindingOperations.GetBindingExpression(target, TextBox.TextProperty);
        Assert.NotNull(expression);
        expression!.UpdateSource();

        // Positive, same instances: proves the binding was live and two-way, simply
        // waiting for the explicit push - not disconnected, not OneWay, not silently
        // dropped.
        Assert.Equal("edited", source.Label);
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
