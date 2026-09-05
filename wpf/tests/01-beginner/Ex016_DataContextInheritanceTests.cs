using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex016_DataContextInheritanceTests : WpfTestContext
{
    // Two hops between root and target on purpose: a single-level tree could not tell
    // "inherited from the ancestor" apart from "inherited from the immediate parent".
    // inner is never asserted on directly - it exists so the inherited value has to
    // pass all the way through an intermediate element rather than reaching target
    // straight from its immediate parent.
    private static (Grid Root, Border Middle, Border Inner, TextBlock Target) BuildTree()
    {
        var root = new Grid();
        var middle = new Border();
        var inner = new Border();
        var target = new TextBlock();

        inner.Child = target;
        middle.Child = inner;
        root.Children.Add(middle);

        return (root, middle, inner, target);
    }

    [WpfFact]
    public void Inherited_DataContext_Flows_Down_Two_Levels_With_No_DataContext_Set_On_The_Target()
    {
        var (root, _, _, target) = BuildTree();
        root.DataContext = new Ex016_PersonSource { Name = "Ada" };

        Ex016_DataContextInheritance.BindName(target);
        Layout(root);
        Pump();

        Assert.Equal("Ada", target.Text);
    }

    [WpfFact]
    public void A_Later_Name_Change_Still_Reaches_The_Target()
    {
        var (root, _, _, target) = BuildTree();
        var person = new Ex016_PersonSource { Name = "Ada" };
        root.DataContext = person;
        Ex016_DataContextInheritance.BindName(target);
        Layout(root);
        Pump();

        person.Name = "Grace";
        Pump();

        // Rules out a one-time copy of the inherited DataContext taken at Bind-time.
        Assert.Equal("Grace", target.Text);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_No_Source_Or_RelativeSource()
    {
        var (root, _, _, target) = BuildTree();
        root.DataContext = new Ex016_PersonSource();

        Ex016_DataContextInheritance.BindName(target);
        Layout(root);
        Pump();

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex016_PersonSource.Name), binding!.Path.Path);
        Assert.Null(binding.Source);
        Assert.Null(binding.RelativeSource);

        // Proves target itself never received a local DataContext - a bypass that
        // copied the ancestor's DataContext onto target directly would show "Local"
        // here instead of "Inherited".
        var source = DependencyPropertyHelper.GetValueSource(target, FrameworkElement.DataContextProperty);
        Assert.Equal(BaseValueSource.Inherited, source.BaseValueSource);
    }

    [WpfFact]
    public void Overriding_DataContext_On_An_Intermediate_Element_Restarts_Inheritance_For_Its_Subtree()
    {
        var (root, middle, _, target) = BuildTree();
        root.DataContext = new Ex016_PersonSource { Name = "Ada" };

        // Bind and observe the un-overridden value FIRST: a bypass that reads whatever
        // DataContext is ambient at Bind-time (instead of leaving target's DataContext
        // alone and relying on live inheritance) would still resolve correctly if the
        // override happened before this point - so the override below must come after
        // the binding is already in place, not before it.
        Ex016_DataContextInheritance.BindName(target);
        Layout(root);
        Pump();
        Assert.Equal("Ada", target.Text);

        Ex016_DataContextInheritance.OverrideDataContext(middle, new Ex016_PersonSource { Name = "Grace" });
        Layout(root);
        Pump();

        // target is two levels under middle and never sets its own DataContext, yet
        // now sees middle's value, not root's - inheritance restarts at middle rather
        // than being blocked by it.
        Assert.Equal("Grace", target.Text);
    }

    [WpfFact]
    public void OverrideDataContext_Writes_A_Real_Local_Value_Through_The_Property_System()
    {
        var (root, middle, _, _) = BuildTree();
        root.DataContext = new Ex016_PersonSource { Name = "Ada" };

        Ex016_DataContextInheritance.OverrideDataContext(middle, new Ex016_PersonSource { Name = "Grace" });

        // A no-op implementation would leave middle's DataContext "Inherited" from
        // root instead of "Local" - this is what makes the restart in the previous
        // test happen at all, checked directly through the property system rather
        // than only inferred from the rendered text.
        var source = DependencyPropertyHelper.GetValueSource(middle, FrameworkElement.DataContextProperty);
        Assert.Equal(BaseValueSource.Local, source.BaseValueSource);
        Assert.Equal("Grace", ((Ex016_PersonSource)middle.DataContext).Name);
    }
}
