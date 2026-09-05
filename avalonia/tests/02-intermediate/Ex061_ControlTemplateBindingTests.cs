using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex061_ControlTemplateBindingTests
{
    private static Ex061_ControlTemplateBinding Show() =>
        ViewHarness.Show(new Ex061_ControlTemplateBinding(), 300, 160);

    private static Ex061_Gauge Gauge(Ex061_ControlTemplateBinding view) =>
        view.FindControl<Ex061_Gauge>("Gauge")!;

    private static TextBlock Caption(Ex061_Gauge gauge) =>
        gauge.GetVisualDescendants().OfType<TextBlock>().Single();

    private static Border Root(Ex061_Gauge gauge) =>
        gauge.GetVisualDescendants().OfType<Border>().First();

    // Mechanism check: the gauge is a TemplatedControl with no visual of its
    // own, so a Template only exists if a ControlTheme targeting its type was
    // actually resolved onto it. Wrapping a Border and a TextBlock around the
    // gauge in the view's own markup instead leaves Template null.
    [AvaloniaFact]
    public void A_ControlTheme_Supplies_The_Gauge_Its_Template()
    {
        var gauge = Gauge(Show());
        Dispatcher.UIThread.RunJobs();

        Assert.NotNull(gauge.Theme);
        Assert.Equal(typeof(Ex061_Gauge), gauge.Theme!.TargetType);
        Assert.NotNull(gauge.Template);
    }

    [AvaloniaFact]
    public void The_Template_Renders_Caption_And_Accent()
    {
        var gauge = Gauge(Show());
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Gauge A", Caption(gauge).Text);
        Assert.Equal(Color.Parse("#FF3366"), (Root(gauge).Background as ISolidColorBrush)?.Color);
    }

    // A literal Text and a literal Background inside the template reproduce
    // everything asserted above. Neither can follow the property changing after
    // the template has already been applied - only a live TemplateBinding does,
    // and this covers both a text and a non-text target property.
    [AvaloniaFact]
    public void Changing_Caption_And_Accent_After_Show_Updates_The_Rendered_Visual()
    {
        var view = Show();
        var gauge = Gauge(view);
        Dispatcher.UIThread.RunJobs();
        var caption = Caption(gauge);
        var root = Root(gauge);

        gauge.Caption = "Gauge B";
        gauge.Accent = new SolidColorBrush(Colors.DarkSlateBlue);
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Gauge B", caption.Text);
        Assert.Equal(Colors.DarkSlateBlue, (root.Background as ISolidColorBrush)?.Color);
    }

    // The discriminator between TemplateBinding and plain Binding, which is the
    // whole subject of the exercise. Measured: the template's children DO
    // inherit the gauge's DataContext, so {Binding Caption} would resolve
    // against the decoy and render "WRONG", while {TemplateBinding Caption}
    // keeps reading the templated parent. The assertion on the TextBlock's own
    // DataContext makes the trap's premise explicit rather than implied - if
    // the decoy never reached the template, this test would prove nothing.
    [AvaloniaFact]
    public void A_Foreign_DataContext_Does_Not_Hijack_The_Caption()
    {
        var view = Show();
        var gauge = Gauge(view);
        Dispatcher.UIThread.RunJobs();
        var caption = Caption(gauge);

        gauge.DataContext = new Ex061_Decoy();
        Dispatcher.UIThread.RunJobs();

        Assert.IsType<Ex061_Decoy>(caption.DataContext);
        Assert.Equal("Gauge A", caption.Text);
    }
}
