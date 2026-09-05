using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex073_CustomLayoutPanelTests
{
    private static Ex073_CustomLayoutPanel Staircase(int children, double step = 5.0)
    {
        var panel = new Ex073_CustomLayoutPanel { Step = step };

        for (var i = 0; i < children; i++)
        {
            panel.Children.Add(new Border { Width = 20, Height = 10 });
        }

        return panel;
    }

    private static Ex073_CustomLayoutPanel Shown(Ex073_CustomLayoutPanel panel)
    {
        ViewHarness.Show(panel, 300, 200);
        Dispatcher.UIThread.RunJobs();
        return panel;
    }

    [AvaloniaFact]
    public void Each_Child_Is_Offset_From_The_One_Before_It()
    {
        var panel = Shown(Staircase(3));

        Assert.Equal(
            [new Rect(0, 0, 20, 10), new Rect(5, 5, 20, 10), new Rect(10, 10, 20, 10)],
            panel.Children.Select(c => c.Bounds));
    }

    // Guards against a hard-coded step of 5.
    [AvaloniaFact]
    public void The_Step_Property_Drives_The_Offsets()
    {
        var panel = Shown(Staircase(3, step: 12));

        Assert.Equal(
            [new Rect(0, 0, 20, 10), new Rect(12, 12, 20, 10), new Rect(24, 24, 20, 10)],
            panel.Children.Select(c => c.Bounds));
    }

    // The union of the staircase, not of the children: 20x10 would ignore the
    // offsets and 60x30 would add them up instead of taking the far corner.
    [AvaloniaFact]
    public void The_Panel_Wants_The_Union_Of_The_Staircase()
    {
        var panel = Staircase(3);

        panel.Measure(new Size(300, 200));

        Assert.Equal(new Size(30, 20), panel.DesiredSize);
    }

    [AvaloniaFact]
    public void An_Empty_Panel_Wants_Nothing()
    {
        var panel = Staircase(0);

        panel.Measure(new Size(300, 200));

        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    // A child that sizes to content has no useful DesiredSize until it has been
    // measured, so an implementation that arranges straight from Width and
    // Height collapses it to nothing. A Border with a fixed-size child is the
    // smallest thing that shows this.
    [AvaloniaFact]
    public void A_Child_That_Sizes_To_Content_Is_Measured_Before_Being_Arranged()
    {
        var panel = new Ex073_CustomLayoutPanel { Step = 4 };
        panel.Children.Add(new Border { Padding = new Thickness(3), Child = new Border { Width = 14, Height = 8 } });
        panel.Children.Add(new Border { Width = 20, Height = 10 });

        Shown(panel);

        Assert.Equal(new Rect(0, 0, 20, 14), panel.Children[0].Bounds);
        Assert.Equal(new Rect(4, 4, 20, 10), panel.Children[1].Bounds);
    }

    // Step is registered with AffectsMeasure, so changing it must re-run layout
    // rather than leave the old arrangement on screen.
    [AvaloniaFact]
    public void Changing_The_Step_Re_Arranges_The_Children()
    {
        var panel = Shown(Staircase(2));

        panel.Step = 25;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(new Rect(25, 25, 20, 10), panel.Children[1].Bounds);
    }

    [AvaloniaFact]
    public void A_Child_Added_After_Layout_Joins_The_Staircase()
    {
        var panel = Shown(Staircase(2));

        panel.Children.Add(new Border { Width = 20, Height = 10 });
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(new Rect(10, 10, 20, 10), panel.Children[2].Bounds);
    }
}
