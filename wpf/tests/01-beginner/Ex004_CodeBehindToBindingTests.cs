using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex004_CodeBehindToBindingTests : WpfTestContext
{
    [WpfFact]
    public void Source_Value_Reaches_The_Target()
    {
        var source = new Ex004_ReadingSource { Label = "inlet" };
        var target = new TextBox();

        Ex004_CodeBehindToBinding.Bind(target, source);
        Layout(target);

        Assert.Equal("inlet", target.Text);
    }

    [WpfFact]
    public void Later_Source_Changes_Still_Reach_The_Target()
    {
        var source = new Ex004_ReadingSource { Label = "inlet" };
        var target = new TextBox();
        Ex004_CodeBehindToBinding.Bind(target, source);
        Layout(target);

        source.Label = "outlet";
        Pump();

        // This is what separates a binding from a one-time copy - and it is why a test
        // that only checked the initial value would be satisfied by `target.Text = "inlet"`.
        Assert.Equal("outlet", target.Text);
    }

    [WpfFact]
    public void Target_Edits_Reach_The_Source_Immediately()
    {
        var source = new Ex004_ReadingSource { Label = "inlet" };
        var target = new TextBox();
        Ex004_CodeBehindToBinding.Bind(target, source);
        Layout(target);

        target.Text = "edited";
        Pump();

        // Immediately, i.e. without focus ever leaving: that is UpdateSourceTrigger.
        Assert.Equal("edited", source.Label);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_The_Expected_Path_Mode_And_Trigger()
    {
        var source = new Ex004_ReadingSource();
        var target = new TextBox();

        Ex004_CodeBehindToBinding.Bind(target, source);

        // Asserting the declaration, not only the observed values: three different
        // implementations produce the same text and only one of them is the exercise.
        var binding = BindingOperations.GetBinding(target, TextBox.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex004_ReadingSource.Label), binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);
    }

    [WpfFact]
    public void The_Source_Is_Reached_Through_The_DataContext()
    {
        var source = new Ex004_ReadingSource();
        var target = new TextBox();

        Ex004_CodeBehindToBinding.Bind(target, source);

        Assert.Same(source, target.DataContext);
    }
}
