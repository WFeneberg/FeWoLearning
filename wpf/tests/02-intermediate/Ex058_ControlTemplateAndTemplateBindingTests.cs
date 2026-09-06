using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex058_ControlTemplateAndTemplateBindingTests : WpfTestContext
{
    [WpfFact]
    public void BuildTemplate_Binds_The_Named_Part_To_HeaderText_Via_TemplatedParent()
    {
        var control = new Ex058_HeaderedControl { HeaderText = "Initial" };
        var template = Ex058_ControlTemplateAndTemplateBinding.BuildTemplate();

        Ex058_ControlTemplateAndTemplateBinding.Retemplate(control, template);

        var part = control.GetPart("PART_Header") as TextBlock;
        Assert.NotNull(part);
        Assert.Equal("Initial", part!.Text);

        // Mechanism check, not just outcome (see wpf/README.md's "writing a test that lies"):
        // the binding itself must actually be a TemplatedParent RelativeSource binding, not a
        // plain Binding against some other source that merely happened to read "Initial" too.
        var expression = part.GetBindingExpression(TextBlock.TextProperty);
        Assert.NotNull(expression);
        Assert.Equal(RelativeSourceMode.TemplatedParent, expression!.ParentBinding.RelativeSource?.Mode);
        Assert.Equal(nameof(Ex058_HeaderedControl.HeaderText), expression.ParentBinding.Path.Path);

        // Against a bypass that binds the part to a plain/wrong source (a literal, a captured
        // local, ElementName, ...): changing HeaderText afterward would never be reflected.
        control.HeaderText = "Changed";
        Pump(DispatcherPriority.DataBind);
        Assert.Equal("Changed", part.Text);
    }

    [WpfFact]
    public void A_Different_HeaderText_And_Control_Also_Tracks_Via_TemplatedParent()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var control = new Ex058_HeaderedControl { HeaderText = "Alpha" };
        var template = Ex058_ControlTemplateAndTemplateBinding.BuildTemplate();

        Ex058_ControlTemplateAndTemplateBinding.Retemplate(control, template);
        var part = (TextBlock)control.GetPart("PART_Header")!;
        Assert.Equal("Alpha", part.Text);

        control.HeaderText = "Omega";
        Pump(DispatcherPriority.DataBind);
        Assert.Equal("Omega", part.Text);
    }

    [WpfFact]
    public void Retemplate_Applies_Immediately_Without_A_Layout_Pass()
    {
        // Against a bypass that only assigns control.Template without ever calling
        // ApplyTemplate(): with no Layout(...)/Pump() call anywhere in this test, the part
        // would still be null.
        var control = new Ex058_HeaderedControl { HeaderText = "NoLayoutNeeded" };
        var template = Ex058_ControlTemplateAndTemplateBinding.BuildTemplate();

        Ex058_ControlTemplateAndTemplateBinding.Retemplate(control, template);

        var part = control.GetPart("PART_Header") as TextBlock;
        Assert.NotNull(part);
        Assert.Equal("NoLayoutNeeded", part!.Text);
    }

    [WpfFact]
    public void Retemplating_With_A_New_Template_Replaces_The_Old_Parts()
    {
        var control = new Ex058_HeaderedControl { HeaderText = "First" };
        var firstTemplate = Ex058_ControlTemplateAndTemplateBinding.BuildTemplate();
        Ex058_ControlTemplateAndTemplateBinding.Retemplate(control, firstTemplate);
        Assert.NotNull(control.GetPart("PART_Header"));

        var secondTemplate = new ControlTemplate(typeof(Ex058_HeaderedControl));
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.Name = "PART_Body";
        factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex058_HeaderedControl.HeaderText))
        {
            RelativeSource = RelativeSource.TemplatedParent,
        });
        secondTemplate.VisualTree = factory;

        Ex058_ControlTemplateAndTemplateBinding.Retemplate(control, secondTemplate);

        Assert.Null(control.GetPart("PART_Header"));
        var body = control.GetPart("PART_Body") as TextBlock;
        Assert.NotNull(body);
        Assert.Equal("First", body!.Text);
    }
}
