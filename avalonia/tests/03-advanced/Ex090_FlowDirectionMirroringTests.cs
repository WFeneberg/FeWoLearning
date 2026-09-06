using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex090_FlowDirectionMirroringTests
{
    private static Control Shown(Control host)
    {
        ViewHarness.ShowWindow(host, 260, 160);
        Dispatcher.UIThread.RunJobs();
        return host;
    }

    private static TextBlock Label(Control host, string name) =>
        host.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Name == name);

    /// <summary>Where the laid-out line begins, which is the observable proof of mirroring.</summary>
    private static double LineStart(TextBlock label) => label.TextLayout.TextLines[0].Start;

    private static double TextWidth(TextBlock label) => label.TextLayout.Width;

    [AvaloniaFact]
    public void The_Label_Inherits_The_Hosts_Direction_Rather_Than_Setting_Its_Own()
    {
        var ltr = Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.LeftToRight));
        var rtl = Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.RightToLeft));

        Assert.Equal(FlowDirection.LeftToRight, Label(ltr, "Label").FlowDirection);
        Assert.Equal(FlowDirection.RightToLeft, Label(rtl, "Label").FlowDirection);
    }

    // Left to right puts the line against the near edge, which is x = 0.
    [AvaloniaFact]
    public void Left_To_Right_Starts_The_Line_At_The_Left_Edge()
    {
        var label = Label(Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.LeftToRight)), "Label");

        Assert.Equal(0, LineStart(label), precision: 6);
    }

    // ...and right to left puts it against the far one. Asserted as the
    // relationship "width minus the text" rather than as a number, because the
    // text's own width depends on font metrics - measured here as 42 in a
    // 120-wide label, giving a start of 78.
    [AvaloniaFact]
    public void Right_To_Left_Starts_The_Line_Flush_With_The_Other_Edge()
    {
        var label = Label(Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.RightToLeft)), "Label");

        Assert.Equal(
            Ex090_FlowDirectionMirroring.LabelWidth - TextWidth(label),
            LineStart(label),
            precision: 6);
    }

    // Stated separately because it is the claim a reader wants: the two directions
    // genuinely differ, and by the width of the slack.
    [AvaloniaFact]
    public void The_Two_Directions_Really_Differ()
    {
        var ltr = Label(Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.LeftToRight)), "Label");
        var rtl = Label(Shown(Ex090_FlowDirectionMirroring.BuildHost(FlowDirection.RightToLeft)), "Label");

        Assert.True(TextWidth(ltr) < Ex090_FlowDirectionMirroring.LabelWidth,
            "the label has to be wider than its text, or there is no slack to mirror into");
        Assert.True(LineStart(rtl) > LineStart(ltr),
            $"expected the right-to-left line to start further along, got {LineStart(rtl)} against {LineStart(ltr)}");
    }

    // The characters move with the line, so this is mirroring rather than an
    // alignment quirk of the line's reported origin.
    [AvaloniaFact]
    public void The_First_Character_Sits_Where_The_Line_Starts()
    {
        foreach (var direction in new[] { FlowDirection.LeftToRight, FlowDirection.RightToLeft })
        {
            var label = Label(Shown(Ex090_FlowDirectionMirroring.BuildHost(direction)), "Label");

            Assert.Equal(LineStart(label), label.TextLayout.HitTestTextPosition(0).X, precision: 6);
        }
    }

    [AvaloniaFact]
    public void An_Explicit_Direction_On_A_Child_Beats_The_Inherited_One()
    {
        var host = Shown(Ex090_FlowDirectionMirroring.BuildMixedHost());

        Assert.Equal(FlowDirection.RightToLeft, Label(host, "Inherited").FlowDirection);
        Assert.Equal(FlowDirection.LeftToRight, Label(host, "OptedOut").FlowDirection);
    }

    // And the opted-out label is laid out left to right while its sibling, in the
    // same host, is not - which is the whole reason the escape hatch exists.
    [AvaloniaFact]
    public void The_Opted_Out_Label_Is_Laid_Out_The_Other_Way_From_Its_Sibling()
    {
        var host = Shown(Ex090_FlowDirectionMirroring.BuildMixedHost());

        Assert.Equal(0, LineStart(Label(host, "OptedOut")), precision: 6);
        Assert.True(LineStart(Label(host, "Inherited")) > 0,
            "the inherited label should be pushed away from the left edge");
    }
}
