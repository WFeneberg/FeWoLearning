using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex074_LayoutCyclesTests : UnoTestContext
{
    private static Ex074_LayoutCycles Panel()
    {
        var panel = new Ex074_LayoutCycles();
        panel.Children.Add(new Border { Width = 30, Height = 20 });
        return Layout(panel, width: 200, height: 200);
    }

    [Fact]
    public void Without_A_Discovery_Nothing_Is_Requested()
    {
        var panel = Panel();

        Layout(panel, width: 200, height: 200);
        Layout(panel, width: 200, height: 200);

        Assert.Equal(0, panel.ReMeasureRequests);
    }

    [Fact]
    public void A_Discovery_Requests_One_Re_Measure()
    {
        var panel = Panel();

        panel.DiscoveredWidth = 80;

        // DiscoveredWidth is a plain property, so nothing is dirty until the test says so -
        // in an app the discovery would happen inside the arrange that follows a real
        // invalidation.
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);

        Assert.Equal(1, panel.ReMeasureRequests);
    }

    [Fact]
    public void The_Re_Measure_Uses_The_Discovered_Width()
    {
        var panel = Panel();

        panel.DiscoveredWidth = 80;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);

        // The arrange requested a measure; this second layout is the pass that honours it.
        Layout(panel, width: 200, height: 200);

        Assert.Equal(80, panel.MeasuredWidth, 1);
    }

    [Fact]
    public void Further_Arranges_Request_Nothing_More()
    {
        var panel = Panel();
        panel.DiscoveredWidth = 80;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);

        for (var i = 0; i < 10; i++)
        {
            panel.InvalidateArrange();
            Layout(panel, width: 200, height: 200);
        }

        // This is the cycle: measure, arrange, invalidate, measure, ... An unguarded panel
        // pins a core at 100% with no exception anywhere to point at it.
        Assert.Equal(1, panel.ReMeasureRequests);
    }

    [Fact]
    public void The_Passes_Stay_Bounded()
    {
        var panel = Panel();
        panel.DiscoveredWidth = 80;

        for (var i = 0; i < 10; i++)
        {
            panel.InvalidateArrange();
            Layout(panel, width: 200, height: 200);
        }

        Assert.True(panel.MeasurePasses <= 3, $"{panel.MeasurePasses} measure passes");
        Assert.True(panel.ArrangePasses <= 12, $"{panel.ArrangePasses} arrange passes");
    }

    [Fact]
    public void A_New_Discovery_Re_Arms_The_Guard()
    {
        var panel = Panel();
        panel.DiscoveredWidth = 80;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);
        Layout(panel, width: 200, height: 200);

        panel.DiscoveredWidth = 120;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);
        Layout(panel, width: 200, height: 200);

        // The guard suppresses repetition, not change. A latch that never re-arms leaves
        // the panel at the first width it ever discovered.
        Assert.Equal(2, panel.ReMeasureRequests);
        Assert.Equal(120, panel.MeasuredWidth, 1);
    }

    [Fact]
    public void A_Discovery_That_Matches_The_Measurement_Requests_Nothing()
    {
        var panel = Panel();

        panel.DiscoveredWidth = panel.MeasuredWidth;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);

        Assert.Equal(0, panel.ReMeasureRequests);
    }

    [Fact]
    public void The_Children_Are_Still_Arranged()
    {
        var panel = Panel();
        var child = (Border)panel.Children[0];

        panel.DiscoveredWidth = 80;
        panel.InvalidateArrange();
        Layout(panel, width: 200, height: 200);
        Layout(panel, width: 200, height: 200);

        Assert.Equal(30, child.ActualWidth, 1);
        Assert.Equal(20, child.ActualHeight, 1);
    }
}
