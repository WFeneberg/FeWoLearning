using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex059_TemplatedControlBasicsTests
{
    private static Ex059_TemplatedControlBasics Arrange(string labelText) =>
        ViewHarness.Show(new Ex059_TemplatedControlBasics { LabelText = labelText }, 200, 80);

    // Mechanism check: a control with no ControlTheme assigned never gets a
    // Template at all - Theme must target this control's own type.
    [AvaloniaFact]
    public void A_ControlTheme_Targeting_This_Type_Is_Applied()
    {
        var control = Arrange("Ada");

        Assert.NotNull(control.Theme);
        Assert.Equal(typeof(Ex059_TemplatedControlBasics), control.Theme!.TargetType);
        Assert.NotNull(control.Template);
    }

    [AvaloniaFact]
    public void Renders_The_Initial_LabelText()
    {
        var control = Arrange("Ada");

        var text = control.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("Ada", text!.Text);
    }

    // The real discriminator: a hard-coded Border+TextBlock reproduces the
    // resting visual above but can never follow LabelText changing, because it
    // was never bound to it in the first place - only a genuine TemplateBinding
    // stays live after the template has already been applied.
    [AvaloniaFact]
    public void Changing_LabelText_After_Show_Updates_The_Rendered_Text()
    {
        var control = Arrange("Ada");
        var text = control.GetVisualDescendants().OfType<TextBlock>().First();

        control.LabelText = "Grace";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Grace", text.Text);
    }
}
