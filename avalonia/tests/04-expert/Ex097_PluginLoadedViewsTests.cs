using System;
using System.Reflection;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex097_PluginLoadedViewsTests
{
    // The assembly is handed in at run time, which is the whole premise. This one
    // happens to be the content assembly, reached the way a real loader would
    // reach a plugin - by name, not through a compile-time reference.
    private static Assembly PluginAssembly() =>
        Assembly.Load(typeof(Ex097_ChartView).Assembly.GetName().Name!);

    private static Ex097_ScanResult Scanned() => Ex097_PluginLoadedViews.Scan(PluginAssembly());

    [AvaloniaFact]
    public void The_Usable_Views_Are_Mapped_From_Their_View_Model_Types()
    {
        var result = Scanned();

        Assert.Equal(typeof(Ex097_ChartView), result.Accepted[typeof(Ex097_ChartViewModel)]);
        Assert.Equal(typeof(Ex097_TableView), result.Accepted[typeof(Ex097_TableViewModel)]);
    }

    // The robustness half, and the reason this exercise is not just a LINQ query:
    // ReactiveUI's own scan throws on this very type, losing everything with it.
    [AvaloniaFact]
    public void The_Unconstructible_View_Is_Skipped_Rather_Than_Fatal()
    {
        var result = Scanned();

        Assert.Contains(nameof(Ex097_BrokenView), result.Skipped);
        Assert.DoesNotContain(typeof(Ex097_BrokenViewModel), result.Accepted.Keys);
    }

    [AvaloniaFact]
    public void Abstract_Candidates_Appear_In_Neither_List()
    {
        var result = Scanned();

        Assert.DoesNotContain(nameof(Ex097_AbstractPluginView), result.Skipped);
        Assert.DoesNotContain(typeof(Ex097_AbstractPluginView), result.Accepted.Values);
    }

    // Nothing without the marker gets picked up, however view-like it looks -
    // ex092's and ex095's views live in this same assembly and implement IViewFor.
    [AvaloniaFact]
    public void Views_Without_The_Plugin_Marker_Are_Ignored()
    {
        var result = Scanned();

        Assert.DoesNotContain(typeof(Ex092_DocumentView), result.Accepted.Values);
        Assert.DoesNotContain(typeof(Ex095_ReportView), result.Accepted.Values);
        Assert.DoesNotContain(nameof(Ex092_DocumentView), result.Skipped);
    }

    [AvaloniaFact]
    public void The_Scan_Finds_Exactly_The_Marked_Views()
    {
        var result = Scanned();

        Assert.Equal(2, result.Accepted.Count);
        Assert.Single(result.Skipped);
    }

    // An accepted mapping has to be usable, which is the point of recording the
    // view type rather than merely counting it.
    [AvaloniaFact]
    public void An_Accepted_View_Can_Actually_Be_Constructed()
    {
        var result = Scanned();

        var instance = Activator.CreateInstance(result.Accepted[typeof(Ex097_ChartViewModel)]);

        Assert.IsType<Ex097_ChartView>(instance);
    }
}
